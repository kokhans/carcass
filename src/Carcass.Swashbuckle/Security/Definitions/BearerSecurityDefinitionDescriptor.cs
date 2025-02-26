﻿// MIT License
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

namespace Carcass.Swashbuckle.Security.Definitions;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides an extension method for configuring Swagger with a Bearer authentication security definition.
/// </summary>
public static class BearerSecurityDefinitionDescriptor
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Adds a Bearer authentication definition to the Swagger configuration.
    /// </summary>
    /// <param name="swaggerOptions">
    ///     The SwaggerGenOptions instance to which the Bearer authentication definition will be
    ///     added.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="swaggerOptions" /> is null.</exception>
    public static void AddBearerAuthenticationDefinition(this SwaggerGenOptions swaggerOptions)
    {
        ArgumentVerifier.NotNull(swaggerOptions, nameof(swaggerOptions));

        swaggerOptions.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Description = "Bearer authentication.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            }
        );
    }
}