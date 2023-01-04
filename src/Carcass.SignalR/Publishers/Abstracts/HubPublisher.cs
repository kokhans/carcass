// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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
using Carcass.SignalR.Hubs.Abstracts;
using Carcass.SignalR.Messages.Abstracts;
using Microsoft.AspNetCore.SignalR;

namespace Carcass.SignalR.Publishers.Abstracts;

public abstract class HubPublisher<THub, TMessage>
    where THub : AuthorizeHub
    where TMessage : IMessage
{
    protected readonly IHubContext<THub> HubContext;

    protected HubPublisher(IHubContext<THub> hubContext)
    {
        ArgumentVerifier.NotNull(hubContext, nameof(hubContext));

        HubContext = hubContext;
    }

    public abstract Task SendAsync(TMessage message, CancellationToken cancellationToken = default);
}