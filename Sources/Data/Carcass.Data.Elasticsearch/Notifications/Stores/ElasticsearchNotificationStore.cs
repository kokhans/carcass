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
using Carcass.Data.Core.Commands.Notifications.Abstracts;
using Carcass.Data.Core.Commands.Notifications.Stores.Abstracts;
using Carcass.Data.Elasticsearch.Conductors.Abstracts;
using Carcass.Data.Elasticsearch.Notifications.Abstracts;
using Carcass.Data.Elasticsearch.Options;
using Microsoft.Extensions.Options;

namespace Carcass.Data.Elasticsearch.Notifications.Stores;

public sealed class ElasticsearchNotificationStore : INotificationStore
{
    private readonly IElasticsearchConductor _elasticsearchConductor;
    private readonly IOptionsMonitor<ElasticsearchOptions> _optionsMonitor;

    public ElasticsearchNotificationStore(
        IElasticsearchConductor elasticsearchConductor,
        IOptionsMonitor<ElasticsearchOptions> optionsMonitor
    )
    {
        ArgumentVerifier.NotNull(elasticsearchConductor, nameof(elasticsearchConductor));
        ArgumentVerifier.NotNull(optionsMonitor, nameof(optionsMonitor));

        _elasticsearchConductor = elasticsearchConductor;
        _optionsMonitor = optionsMonitor;
    }

    public async Task SaveNotificationAsync(INotification notification, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(notification, nameof(notification));

        if (_optionsMonitor.CurrentValue.Audit is null)
            throw new InvalidOperationException("Elasticsearch audit options is not configured.");

        if (notification is IElasticsearchNotification)
            await _elasticsearchConductor.Instance
                .IndexAsync(notification, id => id.Index(_optionsMonitor.CurrentValue.Audit.Index), cancellationToken);
    }
}