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
using Carcass.Logging.Adapters.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Carcass.Logging.Adapters;

/// <summary>
///     Provides functionality to create logger adapter instances by utilizing a service scope factory.
/// </summary>
public sealed class LoggerAdapterFactory : ILoggerAdapterFactory
{
    /// <summary>
    ///     A factory responsible for creating service scopes, used to provide scoped services.
    /// </summary>
    /// <remarks>
    ///     The <c>IServiceScopeFactory</c> allows the creation of <c>IServiceScope</c> instances,
    ///     which provide scoped access to service dependencies. This is useful for resolving services
    ///     within controlled scopes.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <c>IServiceScopeFactory</c> instance is not provided during initialization.
    /// </exception>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    ///     Responsible for creating instances of logger adapters for specified category names.
    /// </summary>
    public LoggerAdapterFactory(IServiceScopeFactory serviceScopeFactory)
    {
        ArgumentVerifier.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    ///     Creates an instance of a logger adapter for the specified category type.
    /// </summary>
    /// <typeparam name="TCategoryName">The type used to define the logging category.</typeparam>
    /// <returns>A logger adapter instance specific to the provided category type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if required services for the logger adapter creation are not available in the service provider.
    /// </exception>
    public LoggerAdapter<TCategoryName> CreateLoggerAdapter<TCategoryName>()
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        ILoggerFactory loggerFactory = serviceScope.ServiceProvider.GetRequiredService<ILoggerFactory>();

        ILogger<TCategoryName> logger = loggerFactory.CreateLogger<TCategoryName>();

        return new LoggerAdapter<TCategoryName>(logger);
    }

    /// <summary>
    ///     Creates an instance of a logger adapter using the provided logger.
    /// </summary>
    /// <param name="logger">
    ///     The logger instance to be wrapped by the logger adapter.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="LoggerAdapter" /> that wraps the specified logger.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided <paramref name="logger" /> is null.
    /// </exception>
    public LoggerAdapter CreateLoggerAdapter(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        return new LoggerAdapter(logger);
    }

    /// <summary>
    ///     Creates a logger adapter instance for the specified category name.
    /// </summary>
    /// <param name="categoryName">
    ///     The name of the category for which the logger adapter should be created.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="LoggerAdapter" /> initialized with the specified category name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="categoryName" /> is <c>null</c>.
    /// </exception>
    public LoggerAdapter CreateLoggerAdapter(string categoryName)
    {
        ArgumentNullException.ThrowIfNull(categoryName);

        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        ILoggerFactory loggerFactory = serviceScope.ServiceProvider.GetRequiredService<ILoggerFactory>();

        return new LoggerAdapter(loggerFactory, categoryName);
    }
}