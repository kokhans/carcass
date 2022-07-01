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

using Carcass.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Carcass.Mvc.Razor.Rendering.Renderers;

public sealed class RazorViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider
    )
    {
        ArgumentVerifier.NotNull(viewEngine, nameof(viewEngine));
        ArgumentVerifier.NotNull(tempDataProvider, nameof(tempDataProvider));
        ArgumentVerifier.NotNull(serviceProvider, nameof(serviceProvider));

        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderAsync<T>(
        string viewPath,
        T model,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(viewPath, nameof(viewPath));
        ArgumentVerifier.NotNull(model, nameof(model));

        DefaultHttpContext defaultHttpContext = new()
        {
            RequestServices = _serviceProvider
        };
        ActionContext actionContext = new(defaultHttpContext, new RouteData(), new ActionDescriptor());

        IView? view;
        ViewEngineResult viewEngineResult = _viewEngine.GetView(null, viewPath, true);
        if (viewEngineResult.Success)
            view = viewEngineResult.View;
        else
            throw new InvalidOperationException($"View {viewPath} not found.");

        await using StringWriter stringWriter = new();
        ViewDataDictionary<T> viewDataDictionary = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };
        TempDataDictionary tempDataDictionary = new(actionContext.HttpContext, _tempDataProvider);
        ViewContext viewContext = new(
            actionContext,
            view,
            viewDataDictionary,
            tempDataDictionary,
            stringWriter,
            new HtmlHelperOptions()
        );
        await view.RenderAsync(viewContext);

        return stringWriter.ToString();
    }
}