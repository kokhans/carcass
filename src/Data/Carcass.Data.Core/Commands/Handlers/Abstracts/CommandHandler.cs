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
using Carcass.Data.Core.Commands.Abstracts;
using Carcass.Data.Core.Commands.Notifications.Dispatchers.Abstracts;
using Carcass.Data.Core.Commands.Validators.Abstracts;
using Carcass.Data.Core.Sessions.Abstracts;

// ReSharper disable UnusedTypeParameter

namespace Carcass.Data.Core.Commands.Handlers.Abstracts;

public interface ICommandHandler<in TCommand, TCommandResult, TCommandValidator>
    where TCommand : class, ICommand<TCommandResult>
    where TCommandValidator : class, ICommandValidator<TCommand, TCommandResult>
{
    Task<TCommandResult?> HandleCommandAsync(TCommand command, CancellationToken cancellationToken = default);
}

public abstract class CommandHandler<TCommand, TCommandResult, TCommandValidator>
    : ICommandHandler<TCommand, TCommandResult, TCommandValidator>
    where TCommand : class, ICommand<TCommandResult>
    where TCommandValidator : class, ICommandValidator<TCommand, TCommandResult>
{
    private readonly TCommandValidator _commandValidator;

    protected CommandHandler(
        TCommandValidator commandValidator,
        INotificationDispatcher notificationDispatcher
    )
    {
        ArgumentVerifier.NotNull(commandValidator, nameof(commandValidator));
        ArgumentVerifier.NotNull(notificationDispatcher, nameof(notificationDispatcher));

        _commandValidator = commandValidator;
        NotificationDispatcher = notificationDispatcher;
    }

    protected readonly INotificationDispatcher NotificationDispatcher;

    public virtual async Task<TCommandResult?> HandleCommandAsync(
        TCommand command,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(command, nameof(command));

        await _commandValidator.ValidateCommandAsync(command, cancellationToken);

        return default;
    }
}

public abstract class CommandHandler<TCommand, TCommandResult, TCommandValidator, TSession>
    : CommandHandler<TCommand, TCommandResult, TCommandValidator>
    where TCommand : class, ICommand<TCommandResult>
    where TCommandValidator : class, ICommandValidator<TCommand, TCommandResult>
    where TSession : class, ISession
{
    protected CommandHandler(
        TCommandValidator commandValidator,
        INotificationDispatcher notificationDispatcher,
        TSession session
    ) : base(commandValidator, notificationDispatcher)
    {
        ArgumentVerifier.NotNull(session, nameof(session));

        Session = session;
    }

    protected TSession Session { get; }
}