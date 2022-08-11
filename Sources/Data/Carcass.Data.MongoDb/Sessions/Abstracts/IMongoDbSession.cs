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

using System.Linq.Expressions;
using Carcass.Data.Core.Sessions.Abstracts;
using Carcass.Data.MongoDb.Entities.Abstracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Carcass.Data.MongoDb.Sessions.Abstracts;

public interface IMongoDbSession : ITransactionalSession<BsonDocument>
{
    Task BeginTransactionAsync(
        ClientSessionOptions? clientSessionOptions = default,
        TransactionOptions? transactionOptions = default,
        CancellationToken cancellationToken = default
    );

    Task CreateAsync<TDocument>(
        string collectionName,
        TDocument document,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task CreateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task UpdateAsync<TDocument>(
        string collectionName,
        TDocument document,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task DeleteAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task DeleteAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task DeleteAsync<TDocument>(string collectionName, ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task DeleteAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task<TDocument> GetByIdAsync<TDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task<TDocument> GetByIdAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task<TDocument> GetByIdAsync<TDocument>(
        string collectionName,
        ObjectId id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task<TDocument> GetByIdAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    Task<IList<TDocument>> QueryAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task<IList<TDocument>> QueryAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task<long> CountAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    Task<long> CountAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;
}