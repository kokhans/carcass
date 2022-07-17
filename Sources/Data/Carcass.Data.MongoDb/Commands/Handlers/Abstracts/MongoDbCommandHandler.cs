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

using Carcass.Data.Core.Commands.Abstracts;
using Carcass.Data.Core.Commands.Handlers.Abstracts;
using Carcass.Data.Core.Commands.Notifications.Dispatchers.Abstracts;
using Carcass.Data.Core.Commands.Validators.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;

namespace Carcass.Data.MongoDb.Commands.Handlers.Abstracts;

public abstract class MongoDbCommandHandler<TCommand, TCommandResult, TCommandValidator>
    : CommandHandler<TCommand, TCommandResult, TCommandValidator, IMongoDbSession>
    where TCommand : class, ICommand<TCommandResult>
    where TCommandValidator : class, ICommandValidator<TCommand, TCommandResult>
{
    protected MongoDbCommandHandler(
        IMongoDbSession session,
        TCommandValidator commandValidator,
        INotificationDispatcher notificationDispatcher
    ) : base(commandValidator, notificationDispatcher, session)
    {
    }
}