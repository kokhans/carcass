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

using Carcass.Core;
using Carcass.Data.Core.Audit.Abstracts;
using Carcass.Data.Elasticsearch.Options;
using Elastic.Clients.Elasticsearch;
using MediatR;
using Microsoft.Extensions.Options;

namespace Carcass.Data.Elasticsearch.Audit;

/// <summary>
///     Handles notifications related to audit entries by indexing them into an Elasticsearch index.
///     This class is responsible for processing <see cref="IAuditEntryNotification" /> instances
///     and sending the data to the configured Elasticsearch server.
/// </summary>
public sealed class ElasticsearchAuditEntryNotificationHandler : INotificationHandler<IAuditEntryNotification>
{
    /// <summary>
    ///     Represents an instance of the Elasticsearch client used for interacting with an Elasticsearch cluster.
    ///     This client provides operations such as indexing, searching, and managing data in the Elasticsearch system.
    /// </summary>
    /// <remarks>
    ///     The client is configured to operate within the context of an audit entry notification handler,
    ///     enabling the indexing of audit data into a predefined Elasticsearch index.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the client instance is not properly initialized before use.
    /// </exception>
    private readonly ElasticsearchClient _elasticsearchClient;

    /// <summary>
    ///     Provides access to the configuration options for Elasticsearch, encapsulated in the
    ///     <see cref="ElasticsearchOptions" /> instance.
    ///     Used to retrieve and apply the settings related to Elasticsearch operations such as audits.
    /// </summary>
    /// <remarks>
    ///     This variable holds an implementation of <see cref="IOptions{TOptions}" />, where TOptions is
    ///     <see cref="ElasticsearchOptions" />.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the provided <see cref="IOptions{ElasticsearchOptions}" /> instance
    ///     is null upon initialization.
    /// </exception>
    private readonly IOptions<ElasticsearchOptions> _optionsAccessor;

    /// <summary>
    ///     Handles notifications of type <see cref="IAuditEntryNotification" /> and processes audit entries
    ///     asynchronously by interacting with Elasticsearch.
    /// </summary>
    public ElasticsearchAuditEntryNotificationHandler(
        ElasticsearchClient elasticsearchClient,
        IOptions<ElasticsearchOptions> optionsAccessorAccessor
    )
    {
        ArgumentVerifier.NotNull(elasticsearchClient, nameof(elasticsearchClient));
        ArgumentVerifier.NotNull(optionsAccessorAccessor, nameof(optionsAccessorAccessor));

        _elasticsearchClient = elasticsearchClient;
        _optionsAccessor = optionsAccessorAccessor;
    }

    /// <summary>
    ///     Processes an audit entry notification and sends it to Elasticsearch for indexing.
    /// </summary>
    /// <param name="notification">
    ///     The audit entry notification containing information about the audited entity.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token to observe while waiting for the asynchronous operation to be completed. Defaults to None.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided notification is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the cancellation token.
    /// </exception>
    public async Task Handle(IAuditEntryNotification notification, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(notification, nameof(notification));

        await _elasticsearchClient.IndexAsync(
            notification,
            id => id.Index(_optionsAccessor.Value.Audit!.Index),
            cancellationToken
        );
    }
}