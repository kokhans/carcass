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

using System.Collections;
using System.Linq.Expressions;
using Carcass.Core;
using Carcass.Data.MongoDb.Entities.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Carcass.Data.MongoDb.Sessions;

public sealed class MongoDbSession : IMongoDbSession
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDatabase;
    private IClientSessionHandle? _clientSessionHandle;

    public MongoDbSession(MongoClient mongoClient, IMongoDatabase mongoDatabase)
    {
        ArgumentVerifier.NotNull(mongoClient, nameof(mongoClient));
        ArgumentVerifier.NotNull(mongoDatabase, nameof(mongoDatabase));

        _mongoClient = mongoClient;
        _mongoDatabase = mongoDatabase;
    }

    public BsonDocument? TransactionId { get; private set; }

    public async Task BeginTransactionAsync(
        ClientSessionOptions? clientSessionOptions = default,
        TransactionOptions? transactionOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        _clientSessionHandle =
            await _mongoClient.StartSessionAsync(clientSessionOptions, cancellationToken);
        TransactionId = _clientSessionHandle.ServerSession.Id;
        _clientSessionHandle.StartTransaction(transactionOptions);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (_clientSessionHandle is null)
                throw new InvalidOperationException(
                    "The transaction could not be committed due to it is not started."
                );

            await _clientSessionHandle.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_clientSessionHandle is null)
            throw new InvalidOperationException(
                "The transaction could not be rollback due to it is not started."
            );

        await _clientSessionHandle.AbortTransactionAsync(cancellationToken);
    }

    public async Task CreateAsync<TDocument>(
        string collectionName,
        TDocument document,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(document, nameof(document));

        await _mongoDatabase
            .GetCollection<TDocument>(collectionName)
            .InsertOneAsync(document, null, cancellationToken);
    }

    public async Task CreateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(document, nameof(document));

        await CreateAsync(typeof(TDocument).Name, document, cancellationToken);
    }

    public async Task UpdateAsync<TDocument>(
        string collectionName,
        TDocument document,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(document, nameof(document));

        FilterDefinition<TDocument> filterDefinition = Builders<TDocument>.Filter.Eq(d => d.Id, document.Id);
        await _mongoDatabase
            .GetCollection<TDocument>(collectionName)
            .FindOneAndUpdateAsync(
                filterDefinition,
                new ObjectUpdateDefinition<TDocument>(document),
                cancellationToken: cancellationToken
            );
    }

    public async Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(document, nameof(document));

        await UpdateAsync(typeof(TDocument).Name, document, cancellationToken);
    }

    public async Task DeleteAsync<TDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(id, nameof(id));

        await DeleteAsync<TDocument>(collectionName, new ObjectId(id), cancellationToken);
    }

    public async Task DeleteAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(id, nameof(id));

        await DeleteAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    public async Task DeleteAsync<TDocument>(
        string collectionName,
        ObjectId id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotDefault(id, nameof(id));

        if (((IList) typeof(TDocument).GetInterfaces()).Contains(typeof(ISoftDeletableDocument)))
        {
            TDocument document = await GetByIdAsync<TDocument>(collectionName, id, cancellationToken);
            ((ISoftDeletableDocument) document).IsDeleted = true;
            await UpdateAsync(collectionName, document, cancellationToken);

            return;
        }

        await _mongoDatabase
            .GetCollection<TDocument>(collectionName)
            .DeleteOneAsync(d => d.Id == id, cancellationToken);
    }

    public async Task DeleteAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        await DeleteAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    public async Task<TDocument> GetByIdAsync<TDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(id, nameof(id));

        return await GetByIdAsync<TDocument>(collectionName, new ObjectId(id), cancellationToken);
    }

    public async Task<TDocument> GetByIdAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(id, nameof(id));

        return await GetByIdAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    public async Task<TDocument> GetByIdAsync<TDocument>(
        string collectionName,
        ObjectId id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotDefault(id, nameof(id));

        FilterDefinition<TDocument> filterDefinition = Builders<TDocument>.Filter.Eq(d => d.Id, id);

        if (((IList) typeof(TDocument).GetInterfaces()).Contains(typeof(ISoftDeletableDocument)))
            filterDefinition &= Builders<TDocument>.Filter.Eq(nameof(ISoftDeletableDocument.IsDeleted), false);

        IAsyncCursor<TDocument> cursor = await _mongoDatabase
            .GetCollection<TDocument>(collectionName)
            .FindAsync(filterDefinition, cancellationToken: cancellationToken);

        return cursor.SingleOrDefault(cancellationToken);
    }

    public async Task<TDocument> GetByIdAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        return await GetByIdAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    public async Task<IList<TDocument>> QueryAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));

        IMongoCollection<TDocument> collection =
            _mongoDatabase.GetCollection<TDocument>(collectionName);

        if (filter is null)
            return await collection.Find(_ => true).ToListAsync(cancellationToken);

        return await collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IList<TDocument>> QueryAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    )
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await QueryAsync(typeof(TDocument).Name, filter, cancellationToken);
    }

    public async Task<long> CountAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));

        FilterDefinition<TDocument> filterDefinition = Builders<TDocument>.Filter.Empty;

        if (((IList) typeof(TDocument).GetInterfaces()).Contains(typeof(ISoftDeletableDocument)))
            filterDefinition &= Builders<TDocument>.Filter.Eq(nameof(ISoftDeletableDocument.IsDeleted), false);

        if (filter is not null)
            filterDefinition &= filter;

        return await _mongoDatabase
            .GetCollection<TDocument>(collectionName)
            .CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);
    }

    public async Task<long> CountAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await CountAsync(typeof(TDocument).Name, filter, cancellationToken);
    }
}