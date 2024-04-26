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

using Carcass.Logging.Adapters.Abstracts;
using Microsoft.Extensions.Logging;

namespace Carcass.Logging.Adapters;

/// <summary>
///     Provides a generic adapter for structured logging operations,
///     allowing integration with the Microsoft.Extensions.Logging framework.
/// </summary>
/// <typeparam name="TCategoryName">
///     The type whose name is used as the logging category.
/// </typeparam>
public sealed class LoggerAdapter<TCategoryName>(ILogger<TCategoryName> logger) : LoggerAdapterBase(logger);

/// <summary>
///     Provides a non-generic adapter for structured logging operations,
///     allowing integration with Microsoft's logging abstractions.
/// </summary>
public sealed class LoggerAdapter : LoggerAdapterBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LoggerAdapter" /> class using a specific <see cref="ILogger" />.
    ///     This facilitates interaction with the logging infrastructure.
    /// </summary>
    /// <param name="logger">The logger implementation used for logging operations.</param>
    public LoggerAdapter(ILogger logger) : base(logger)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LoggerAdapter" /> class with a category name
    ///     using the provided <see cref="ILoggerFactory" />.
    /// </summary>
    /// <param name="loggerFactory">The factory used to create the logger instance.</param>
    /// <param name="categoryName">The category name for the logger.</param>
    public LoggerAdapter(ILoggerFactory loggerFactory, string categoryName)
        : base(loggerFactory, categoryName)
    {
    }
}