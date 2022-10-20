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

using Carcass.Cli.Logging.Spectre.Loggers.Abstracts;
using Carcass.Cli.Logging.Spectre.Themes.Abstracts;
using Carcass.Core;
using Spectre.Console;

namespace Carcass.Cli.Logging.Spectre.Loggers;

public sealed class SpectreCliLogger : ISpectreCliLogger
{
    private readonly SpectreCliLoggerTheme _theme;

    public SpectreCliLogger(SpectreCliLoggerTheme theme)
    {
        ArgumentVerifier.NotNull(theme, nameof(theme));

        _theme = theme;
    }

    public void LogInformation(string formatString, params object?[] args) =>
        AnsiConsole.MarkupLine(_theme.GetLogInformationMarkup(formatString, args));

    public void LogWarning(string formatString, params object?[] args) =>
        AnsiConsole.MarkupLine(_theme.GetLogWarningMarkup(formatString, args));

    public void LogError(Exception exception) => AnsiConsole.WriteException(exception);

    public void LogError(string formatString, params object?[] args) =>
        AnsiConsole.MarkupLine(_theme.GetLogErrorMarkup(formatString, args));

    public async Task StartLogStatus(
        string status,
        Func<SpectreCliLoggerStatusContext, Task> action,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(status, nameof(status));
        ArgumentVerifier.NotNull(action, nameof(action));

        await AnsiConsole
            .Status()
            .Spinner(Spinner.Known.SimpleDotsScrolling)
            .StartAsync(status, async sc => await action.Invoke(new SpectreCliLoggerStatusContext(sc)));
    }

    public void LogFiglet(string text, Action<FigletText> configure)
    {
        ArgumentVerifier.NotNull(text, nameof(text));
        ArgumentVerifier.NotNull(configure, nameof(configure));

        FigletText figletText = new(text);
        configure(figletText);

        AnsiConsole.Write(figletText);
    }

    public void LogRule(string? text = default, Action<Rule>? configure = default)
    {
        Rule? rule;

        if (string.IsNullOrWhiteSpace(text))
            rule = new Rule();
        else
        {
            rule = new Rule(text);
            configure?.Invoke(rule);
        }

        AnsiConsole.Write(rule);
    }
}