// MIT License
//
// Copyright (c) 2022-2025 Serhii Kokhan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using Carcass.Azure.Functions.Accessors.Abstracts;
using Carcass.Core;
using Carcass.Core.Helpers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

// ReSharper disable ClassNeverInstantiated.Global

namespace Carcass.Firebase.AzureFunctions.Middlewares;

/// <summary>
///     Middleware for authenticating HTTP-triggered Azure Functions using Firebase.
/// </summary>
public sealed class FirebaseAzureFunctionsAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    /// <summary>
    ///     Represents the key used to store and retrieve the <see cref="ClaimsPrincipal" /> instance
    ///     within the function context's items collection.
    /// </summary>
    /// <remarks>
    ///     This constant is used as an identifier for associating user claims data with the current function execution
    ///     context.
    /// </remarks>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown if the key is not present in the function context when attempting to retrieve the
    ///     <see cref="ClaimsPrincipal" />.
    /// </exception>
    private const string ClaimsPrincipalKey = "UserClaimsPrincipal";

    /// <summary>
    ///     Represents the Firebase application instance used for interacting with Firebase services,
    ///     such as authenticating and verifying Firebase tokens.
    /// </summary>
    private readonly FirebaseApp _firebaseApp;

    /// <summary>
    ///     Provides access to the current <see cref="Microsoft.Azure.Functions.Worker.FunctionContext" />
    ///     during the execution of an Azure Function.
    /// </summary>
    private readonly IFunctionContextAccessor _functionContextAccessor;

    /// <summary>
    ///     Middleware for authenticating Firebase requests within Azure Functions.
    /// </summary>
    public FirebaseAzureFunctionsAuthenticationMiddleware(
        FirebaseApp firebaseApp,
        IFunctionContextAccessor functionContextAccessor)
    {
        ArgumentVerifier.NotNull(firebaseApp, nameof(firebaseApp));
        ArgumentVerifier.NotNull(functionContextAccessor, nameof(functionContextAccessor));

        _firebaseApp = firebaseApp;
        _functionContextAccessor = functionContextAccessor;
    }

    /// <summary>
    ///     Middleware to handle Firebase authentication for Azure Functions.
    ///     Validates Firebase ID tokens from the authorization header and attaches the ClaimsPrincipal to the FunctionContext.
    /// </summary>
    /// <param name="context">The current <see cref="FunctionContext" /> for the Azure Function.</param>
    /// <param name="next">The delegate to invoke the next middleware in the execution pipeline.</param>
    /// <returns>A task representing the asynchronous operation of the middleware.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="FunctionContext" /> is not available.</exception>
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        HttpRequestData? httpRequest;

        FunctionContext? functionContext = _functionContextAccessor.FunctionContext;
        if (functionContext is null)
        {
            const string message = "FunctionContext is not available.";

            httpRequest = await context.GetHttpRequestDataAsync();
            if (httpRequest is null)
                throw new InvalidOperationException(message);

            HttpResponseData response = httpRequest.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync(message);
            httpRequest.FunctionContext.Features.Set(response);

            return;
        }

        // Check if the function allows anonymous access.
        if (IsAnonymousAllowed(functionContext))
        {
            await next(functionContext); // Bypass authentication.
            return;
        }

        // Retrieve the HTTP request (for HTTP-triggered functions).
        httpRequest = await functionContext.GetHttpRequestDataAsync();
        if (httpRequest is not null)
        {
            // Validate authentication headers.
            if (!httpRequest.Headers.TryGetValues("Authorization", out IEnumerable<string>? authHeaders))
            {
                await SendUnauthorizedResponseAsync(httpRequest, "Authorization header is missing.");
                return;
            }

            string? authorizationHeader = authHeaders.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
            {
                await SendUnauthorizedResponseAsync(httpRequest, "Invalid or missing Bearer token.");
                return;
            }

            string bearerToken = authorizationHeader["Bearer ".Length..];
            FirebaseToken? firebaseToken;
            try
            {
                firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(bearerToken);
            }
            catch (Exception ex)
            {
                await SendUnauthorizedResponseAsync(httpRequest, $"Invalid token: {ex.Message}");
                return;
            }

            // Create ClaimsPrincipal from Firebase token claims.
            ClaimsPrincipal claimsPrincipal = CreateClaimsPrincipal(firebaseToken);

            // Store the ClaimsPrincipal in the FunctionContext.
            functionContext.Items[ClaimsPrincipalKey] = claimsPrincipal;
        }

        // Pass execution to the next middleware in the pipeline
        await next(functionContext);
    }

    /// <summary>
    ///     Determines whether the specified function allows anonymous access by checking
    ///     for the presence of <see cref="AuthorizeAttribute" /> on the function's method definition.
    /// </summary>
    /// <param name="context">The <see cref="FunctionContext" /> representing the function execution context.</param>
    /// <returns>
    ///     True if the function allows anonymous access; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the given <paramref name="context" /> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the method information cannot be retrieved for the specified function.
    /// </exception>
    private static bool IsAnonymousAllowed(FunctionContext context)
    {
        MethodInfo? methodInfo = GetMethodInfo(context.FunctionDefinition.EntryPoint, context.FunctionDefinition.Name);
        if (methodInfo is null)
            return false;

        return methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length == 0;
    }

    /// <summary>
    ///     Retrieves the <see cref="MethodInfo" /> for a specified method using its entry point and function name.
    /// </summary>
    /// <param name="entryPoint">
    ///     The entry point of the function, typically in the format "Namespace.Class.Method".
    /// </param>
    /// <param name="functionName">
    ///     The name of the method to retrieve within the specified class.
    /// </param>
    /// <returns>
    ///     A <see cref="MethodInfo" /> object for the specified method if found; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <paramref name="entryPoint" /> is in an invalid format or if the corresponding type cannot be found.
    /// </exception>
    private static MethodInfo? GetMethodInfo(string entryPoint, string functionName)
    {
        int lastDotIndex = entryPoint.LastIndexOf('.');
        if (lastDotIndex == -1)
        {
            throw new InvalidOperationException(
                $"Invalid entry point format: {entryPoint}. Expected format 'Namespace.Class.Method'.");
        }

        // Extract the type's fully qualified name (Namespace.ClassName)
        string typeName = entryPoint[..lastDotIndex]; // "Carcass.Sample.AzureFunctions.Functions"

        ReadOnlyCollection<Assembly> assemblies = AssemblyHelper.GetLoadedAssemblies();
        Type? type = assemblies.Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type != null);
        if (type is null)
            throw new InvalidOperationException($"Type '{typeName}' could not be found.");

        return type.GetMethod(functionName);
    }


    /// <summary>
    ///     Creates a <see cref="ClaimsPrincipal" /> from the claims in a Firebase token.
    /// </summary>
    /// <param name="firebaseToken">The Firebase token containing the user's claims.</param>
    /// <returns>A <see cref="ClaimsPrincipal" /> representing the user's identity and claims.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="firebaseToken" /> is null.</exception>
    private static ClaimsPrincipal CreateClaimsPrincipal(FirebaseToken firebaseToken) =>
        new(new ClaimsIdentity(
            firebaseToken.Claims
                .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty))
                .ToList(),
            "Firebase"
        ));

    /// <summary>
    ///     Sends an unauthorized HTTP response with a specified message.
    /// </summary>
    /// <param name="httpRequest">The HTTP request that triggered the function.</param>
    /// <param name="message">The message to include in the unauthorized response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="httpRequest" /> or <paramref name="message" /> is null.
    /// </exception>
    private static async Task SendUnauthorizedResponseAsync(HttpRequestData httpRequest, string message)
    {
        HttpResponseData response = httpRequest.CreateResponse(HttpStatusCode.Unauthorized);
        await response.WriteStringAsync(message);
        httpRequest.FunctionContext.Features.Set(response); // Signal middleware interception.
    }
}