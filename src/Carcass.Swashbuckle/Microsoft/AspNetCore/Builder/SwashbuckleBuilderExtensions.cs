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

using Carcass.Core;
using Carcass.Swashbuckle.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.Builder;

/// <summary>
///     Provides extension methods for configuring and utilizing Swashbuckle within an application.
/// </summary>
public static class SwashbuckleBuilderExtensions
{
    /// <summary>
    ///     Configures and enables the Carcass Swashbuckle middleware in the application pipeline.
    /// </summary>
    /// <param name="app">
    ///     The application builder used to configure the middleware components.
    /// </param>
    /// <returns>
    ///     The configured application builder to support method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="app" /> argument is null.
    /// </exception>
    public static IApplicationBuilder UseCarcassSwashbuckle(this IApplicationBuilder app)
    {
        ArgumentVerifier.NotNull(app, nameof(app));

        IOptions<SwashbuckleOptions> optionsAccessor =
            app.ApplicationServices.GetRequiredService<IOptions<SwashbuckleOptions>>();

        return app
            .UseSwagger()
            .UseSwaggerUI(suo =>
                {
                    suo.SwaggerEndpoint(
                        $"/swagger/{optionsAccessor.Value.Version}/swagger.json",
                        $"{optionsAccessor.Value.Name} " +
                        $"{optionsAccessor.Value.Version}"
                    );
                    suo.DocExpansion(DocExpansion.None);
                }
            );
    }
}