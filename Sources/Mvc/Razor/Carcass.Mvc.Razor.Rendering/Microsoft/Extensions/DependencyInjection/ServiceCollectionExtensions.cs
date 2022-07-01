// MIT License
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

using System.Diagnostics;
using System.Reflection;
using Carcass.Core;
using Carcass.Mvc.Razor.Rendering.Environments;
using Carcass.Mvc.Razor.Rendering.Renderers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassRazorRendering(
        this IServiceCollection services,
        string inputDirectory,
        string applicationName
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(inputDirectory, nameof(inputDirectory));
        ArgumentVerifier.NotNull(applicationName, nameof(applicationName));

        ApplicationEnvironment? applicationEnvironment = PlatformServices.Default.Application;

        services
            .AddSingleton(applicationEnvironment)
            .AddSingleton<IWebHostEnvironment>(new WebHostEnvironment
                {
                    ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? applicationName,
                    ContentRootPath = inputDirectory,
                    ContentRootFileProvider = new PhysicalFileProvider(inputDirectory),
                    WebRootPath = inputDirectory,
                    WebRootFileProvider = new PhysicalFileProvider(inputDirectory)
                }
            )
            .Configure<MvcRazorRuntimeCompilationOptions>(mrrco =>
                {
                    mrrco.FileProviders.Clear();
                    mrrco.FileProviders.Add(new PhysicalFileProvider(inputDirectory));
                }
            );
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton(new DiagnosticListener("Microsoft.AspNetCore"));
        services.TryAddSingleton<DiagnosticSource>(sp => sp.GetRequiredService<DiagnosticListener>());
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.TryAddSingleton<ConsolidatedAssemblyApplicationPartFactory>();
        services
            .AddLogging()
            .AddHttpContextAccessor()
            .AddMvcCore()
            .AddRazorPages()
            .AddRazorRuntimeCompilation();
        services.AddSingleton<RazorViewRenderer>();

        return services;
    }
}