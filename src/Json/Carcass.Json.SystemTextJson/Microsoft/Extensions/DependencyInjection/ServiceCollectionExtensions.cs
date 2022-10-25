﻿// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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

using System.Text.Json;
using Carcass.Core;
using Carcass.Json.Core.Providers.Abstracts;
using Carcass.Json.SystemTextJson.Providers;
using Carcass.Json.SystemTextJson.Providers.Abstracts;
using Carcass.Json.SystemTextJson.Settings;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassSystemTextJsonProvider(
        this IServiceCollection services,
        JsonSerializerOptions? options = default
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        SystemTextJsonProvider systemTextJsonProvider = new(
            options ?? SystemTextJsonSettings.Defaults()
        );

        return services
            .AddSingleton<IJsonProvider>(systemTextJsonProvider)
            .AddSingleton<ISystemTextJsonProvider>(systemTextJsonProvider);
    }
}