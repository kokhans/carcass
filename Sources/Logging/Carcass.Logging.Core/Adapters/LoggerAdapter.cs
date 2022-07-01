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
using Microsoft.Extensions.Logging;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace Carcass.Logging.Core.Adapters;

public sealed class LoggerAdapter<TCategoryName>
{
    private readonly ILogger<TCategoryName> _logger;

    public LoggerAdapter(ILoggerFactory loggerFactory)
    {
        ArgumentVerifier.NotNull(loggerFactory, nameof(loggerFactory));

        _logger = loggerFactory.CreateLogger<TCategoryName>();
    }

    public void LogTrace(string message) => _logger.Log(LogLevel.Trace, new EventId((int) LogLevel.Trace), message);

    public void LogTrace<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Trace,
        new EventId((int) LogLevel.Trace),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

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

    public void LogDebug(string message) => _logger.Log(LogLevel.Debug, new EventId((int) LogLevel.Debug), message);

    public void LogDebug<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Debug,
        new EventId((int) LogLevel.Debug),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

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

    public void LogInformation(string message) => _logger.Log(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        message
    );

    public void LogInformation<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Information,
        new EventId((int) LogLevel.Information),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

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

    public void LogWarning(string message) => _logger.Log(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        message
    );

    public void LogWarning<T1>(
        string formatString,
        T1 arg1
    ) => LoggerMessage.Define<T1>(
        LogLevel.Warning,
        new EventId((int) LogLevel.Warning),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, null);

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

    public void LogError(string message, Exception? exception = default) => _logger.Log(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        exception,
        message
    );

    public void LogError(Exception exception) => _logger.Log(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        exception,
        null
    );

    public void LogError<T1>(
        string formatString,
        T1 arg1,
        Exception? exception = default
    ) => LoggerMessage.Define<T1>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, exception);

    public void LogError<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, exception);

    public void LogError<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, exception);

    public void LogError<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, exception);

    public void LogError<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, exception);

    public void LogError<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Error,
        new EventId((int) LogLevel.Error),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, exception);

    public void LogCritical(string message, Exception? exception = default) => _logger.Log(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        exception,
        message
    );

    public void LogCritical(Exception exception) => _logger.Log(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        exception,
        null
    );

    public void LogCritical<T1>(
        string formatString,
        T1 arg1,
        Exception? exception = default
    ) => LoggerMessage.Define<T1>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, exception);

    public void LogCritical<T1, T2>(
        string formatString,
        T1 arg1,
        T2 arg2,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, exception);

    public void LogCritical<T1, T2, T3>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, exception);

    public void LogCritical<T1, T2, T3, T4>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3, T4>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, exception);

    public void LogCritical<T1, T2, T3, T4, T5>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, exception);

    public void LogCritical<T1, T2, T3, T4, T5, T6>(
        string formatString,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        Exception? exception = default
    ) => LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(
        LogLevel.Critical,
        new EventId((int) LogLevel.Critical),
        formatString,
        new LogDefineOptions {SkipEnabledCheck = false}
    ).Invoke(_logger, arg1, arg2, arg3, arg4, arg5, arg6, exception);
}