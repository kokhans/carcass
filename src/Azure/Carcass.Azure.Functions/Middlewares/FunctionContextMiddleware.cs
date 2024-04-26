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

using Carcass.Azure.Functions.Accessors.Abstracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassNeverInstantiated.Global

namespace Carcass.Azure.Functions.Middlewares;

/// <summary>
///     Middleware responsible for managing the Azure Function execution context within the application's dependency
///     injection container.
/// </summary>
public sealed class FunctionContextMiddleware : IFunctionsWorkerMiddleware
{
    /// <summary>
    ///     Executes the middleware, providing the current <see cref="FunctionContext" /> to the
    ///     <see cref="IFunctionContextAccessor" /> and continuing the function execution pipeline.
    /// </summary>
    /// <param name="context">The current <see cref="FunctionContext" /> for the Azure Function execution.</param>
    /// <param name="next">
    ///     Delegate representing the next middleware or function in the execution pipeline.
    /// </param>
    /// <returns>An asynchronous <see cref="Task" /> that completes when the execution pipeline has been fully processed.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IFunctionContextAccessor" /> could not be resolved from the dependency injection
    ///     container.
    /// </exception>
    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        IFunctionContextAccessor accessor = context.InstanceServices.GetRequiredService<IFunctionContextAccessor>();
        accessor.FunctionContext = context;

        return next(context);
    }
}