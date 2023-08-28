﻿// MIT License
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
using Carcass.Data.Core.Audit.Abstracts;
using Carcass.Data.Elasticsearch.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Nest;

namespace Carcass.Data.Elasticsearch.Audit;

public sealed class ElasticsearchAuditEntryNotificationHandler : INotificationHandler<IAuditEntryNotification>
{
    private readonly IElasticClient _elasticClient;
    private readonly IOptions<ElasticsearchOptions> _optionsAccessor;

    public ElasticsearchAuditEntryNotificationHandler(
        IElasticClient elasticClient,
        IOptions<ElasticsearchOptions> optionsAccessorAccessor
    )
    {
        ArgumentVerifier.NotNull(elasticClient, nameof(elasticClient));
        ArgumentVerifier.NotNull(optionsAccessorAccessor, nameof(optionsAccessorAccessor));

        _elasticClient = elasticClient;
        _optionsAccessor = optionsAccessorAccessor;
    }

    public async Task Handle(IAuditEntryNotification notification, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(notification, nameof(notification));

        await _elasticClient.IndexAsync(
            notification,
            id => id.Index(_optionsAccessor.Value.Audit!.Index),
            cancellationToken
        );
    }
}