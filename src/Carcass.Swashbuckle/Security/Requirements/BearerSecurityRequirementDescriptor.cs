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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Carcass.Swashbuckle.Security.Requirements;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides an extension method to configure Bearer security requirements for Swagger documentation.
///     This extension is used to add Bearer authentication schemes to the OpenAPI definition.
/// </summary>
public static class BearerSecurityRequirementDescriptor
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Adds a Bearer security requirement to the Swagger documentation options.
    /// </summary>
    /// <param name="swaggerOptions">
    ///     The SwaggerGenOptions instance to which the Bearer security requirement will be added.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="swaggerOptions" /> is null.
    /// </exception>
    public static void AddBearerSecurityRequirement(this SwaggerGenOptions swaggerOptions)
    {
        ArgumentVerifier.NotNull(swaggerOptions, nameof(swaggerOptions));

        swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            }
        );
    }
}