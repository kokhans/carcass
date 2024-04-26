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

using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace Carcass.Logging.Adapters.Abstracts;

/// <summary>
///     Represents a factory for creating instances of logger adapters.
/// </summary>
public interface ILoggerAdapterFactory
{
    /// <summary>
    ///     Creates a new instance of <see cref="LoggerAdapter{TCategoryName}" /> for the specified category type.
    /// </summary>
    /// <typeparam name="TCategoryName">The type of the category name that is used for the logger.</typeparam>
    /// <returns>A new instance of <see cref="LoggerAdapter{TCategoryName}" /> for the given category.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the required logger instance cannot be created.</exception>
    LoggerAdapter<TCategoryName> CreateLoggerAdapter<TCategoryName>();

    /// <summary>
    ///     Creates an instance of a logger adapter for the specified logger.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger" /> instance to adapt. Cannot be null.</param>
    /// <returns>A new instance of <see cref="LoggerAdapter" /> using the provided logger.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="logger" /> parameter is null.</exception>
    LoggerAdapter CreateLoggerAdapter(ILogger logger);

    /// <summary>
    ///     Creates a new instance of <see cref="LoggerAdapter" /> for the provided category name.
    /// </summary>
    /// <param name="categoryName">
    ///     The name of the logging category to associate with the created <see cref="LoggerAdapter" /> instance.
    /// </param>
    /// <returns>
    ///     A new <see cref="LoggerAdapter" /> instance associated with the specified category name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="categoryName" /> argument is null.
    /// </exception>
    LoggerAdapter CreateLoggerAdapter(string categoryName);
}