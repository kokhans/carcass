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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace Carcass.Logging.Adapters.Abstracts;

/// <summary>
///     Represents a base class for logging adapters that provides methods for structured
///     and standard logging at various log levels.
/// </summary>
[SuppressMessage("Usage", "CA2254:Template should be a static expression")]
public abstract class LoggerAdapterBase
{
    /// <summary>
    ///     A private readonly field used for logging operations.
    /// </summary>
    /// <remarks>
    ///     This field is initialized through the constructor of the derived class
    ///     or using an <c>ILoggerFactory</c> to create an <c>ILogger</c> instance
    ///     specific to a given category.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the logger or logger factory provided during initialization is null.
    /// </exception>
    private readonly ILogger _logger;

    /// <summary>
    ///     Provides a base class for logging operations, handling logging at various levels.
    /// </summary>
    protected LoggerAdapterBase(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <summary>
    ///     Provides a base class for a logger adapter, offering methods for structured and trace-level logging.
    /// </summary>
    protected LoggerAdapterBase(ILoggerFactory loggerFactory, string categoryName)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _logger = loggerFactory.CreateLogger(categoryName);
    }

    /// <summary>
    ///     Logs a trace-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogTrace(string message) =>
        _logger.Log(LogLevel.Trace, new EventId((int) LogLevel.Trace), message);

    /// <summary>
    ///     Logs a trace-level message with parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogTrace<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

    /// <summary>
    ///     Logs a trace-level message with two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogTrace<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, null);

    /// <summary>
    ///     Logs a trace-level message with three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogTrace<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, null);

    /// <summary>
    ///     Logs a trace-level message with four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogTrace<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, null);

    /// <summary>
    ///     Logs a trace-level message with five parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogTrace<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, null);

    /// <summary>
    ///     Logs a trace-level message with six parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogTrace<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, null);

    /// <summary>
    ///     Logs a debug-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogDebug(string message) =>
        _logger.Log(LogLevel.Debug, new EventId((int) LogLevel.Debug), message);

    /// <summary>
    ///     Logs a debug-level message with one parameter.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogDebug<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

    /// <summary>
    ///     Logs a debug-level message with two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogDebug<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, null);

    /// <summary>
    ///     Logs a debug-level message with three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogDebug<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, null);

    /// <summary>
    ///     Logs a debug-level message with four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogDebug<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, null);

    /// <summary>
    ///     Logs a debug-level message with five parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogDebug<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, null);

    /// <summary>
    ///     Logs a debug-level message with six parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogDebug<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, null);

    /// <summary>
    ///     Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogInformation(string message) =>
        _logger.Log(
            LogLevel.Information,
            new EventId((int) LogLevel.Information),
            message
        );

    /// <summary>
    ///     Logs an informational message with one parameter.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogInformation<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

    /// <summary>
    ///     Logs an informational message with two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogInformation<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, null);

    /// <summary>
    ///     Logs an informational message with three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogInformation<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, null);

    /// <summary>
    ///     Logs an informational message with four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogInformation<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, null);

    /// <summary>
    ///     Logs an informational message with five parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogInformation<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, null);

    /// <summary>
    ///     Logs an informational message with six parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogInformation<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, null);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogWarning(string message) =>
        _logger.Log(
            LogLevel.Warning,
            new EventId((int) LogLevel.Warning),
            message
        );

    /// <summary>
    ///     Logs a warning message with one parameter.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogWarning<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

    /// <summary>
    ///     Logs a warning message with two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogWarning<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, null);

    /// <summary>
    ///     Logs a warning message with three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogWarning<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, null);

    /// <summary>
    ///     Logs a warning message with four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogWarning<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, null);

    /// <summary>
    ///     Logs a warning message with five parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogWarning<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, null);

    /// <summary>
    ///     Logs a warning message with six parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogWarning<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, null);

    /// <summary>
    ///     Logs an error message with an optional exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    public void LogError(string message, Exception? exception = null) =>
        _logger.Log(
            LogLevel.Error,
            new EventId((int) LogLevel.Error),
            exception,
            message
        );

    /// <summary>
    ///     Logs an error message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    public void LogError(Exception exception) =>
        _logger.Log(
            LogLevel.Error,
            new EventId((int) LogLevel.Error),
            exception,
            string.Empty
        );

    /// <summary>
    ///     Logs an error message with one parameter and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogError<T1>(
        string formatString,
        T1 arg1,
        Exception? exception = null
    ) => LoggerMessage.Define<T1>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, exception);

    /// <summary>
    ///     Logs an error message with two parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogError<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, exception);

    /// <summary>
    ///     Logs an error message with three parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogError<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, exception);

    /// <summary>
    ///     Logs an error message with four parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogError<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, exception);

    /// <summary>
    ///     Logs an error message with five parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogError<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, exception);

    /// <summary>
    ///     Logs an error message with six parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogError<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, exception);

    /// <summary>
    ///     Logs a critical error message with an optional exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    public void LogCritical(string message, Exception? exception = null) =>
        _logger.Log(
            LogLevel.Critical,
            new EventId((int) LogLevel.Critical),
            exception,
            message
        );

    /// <summary>
    ///     Logs a critical error message with an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    public void LogCritical(Exception exception) =>
        _logger.Log(
            LogLevel.Critical,
            new EventId((int) LogLevel.Critical),
            exception,
            string.Empty
        );

    /// <summary>
    ///     Logs a critical error message with one parameter and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogCritical<T1>(
        string formatString,
        T1 arg1,
        Exception? exception = null
    ) => LoggerMessage.Define<T1>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, exception);

    /// <summary>
    ///     Logs a critical error message with two parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogCritical<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, exception);

    /// <summary>
    ///     Logs a critical error message with three parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogCritical<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, exception);

    /// <summary>
    ///     Logs a critical error message with four parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogCritical<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, exception);

    /// <summary>
    ///     Logs a critical error message with five parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogCritical<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, exception);

    /// <summary>
    ///     Logs a critical error message with six parameters and an optional exception.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
    /// <param name="formatString">The message template.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <param name="exception">An optional exception associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is null.</exception>
    public void LogCritical<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        Exception? exception = null
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, exception);
}