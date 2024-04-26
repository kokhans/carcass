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

using System.Collections;
using System.Linq.Expressions;
using Carcass.Core;
using Carcass.Data.MongoDb.Entities.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Carcass.Data.MongoDb.Sessions;

/// <summary>
///     Represents a session for interacting with a MongoDB database, providing
///     functionality for transactions and CRUD operations.
/// </summary>
public sealed class MongoDbSession : IMongoDbSession
{
    /// <summary>
    ///     Represents the MongoDB client used to interface with the MongoDB server.
    ///     Provides access to the MongoDB server for session management
    ///     and database-level operations in the context of this session.
    /// </summary>
    /// <remarks>
    ///     This field is initialized via dependency injection and is used internally
    ///     to start sessions and perform operations on the MongoDB server.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when a null value is provided to the constructor for this client.
    /// </exception>
    private readonly MongoClient _mongoClient;

    /// <summary>
    ///     Represents the private instance of the MongoDB database being accessed by the session.
    ///     Used to perform CRUD operations and manage collections within the specified database.
    /// </summary>
    private readonly IMongoDatabase _mongoDatabase;

    /// <summary>
    ///     Represents the session handle for managing the client session
    ///     established with the MongoDB server.
    /// </summary>
    /// <remarks>
    ///     This variable is used internally to maintain session state and
    ///     handle transactions with the MongoDB server. It is initialized
    ///     when a transaction is begun and is used throughout the transaction's lifecycle.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if operations requiring an active session are invoked without
    ///     the session being properly initialized.
    /// </exception>
    private IClientSessionHandle? _clientSessionHandle;

    /// <summary>
    ///     Represents a session to interact with a MongoDB database.
    ///     Provides methods for transaction management and CRUD operations.
    /// </summary>
    public MongoDbSession(MongoClient mongoClient, IMongoDatabase mongoDatabase)
    {
        ArgumentVerifier.NotNull(mongoClient, nameof(mongoClient));
        ArgumentVerifier.NotNull(mongoDatabase, nameof(mongoDatabase));

        _mongoClient = mongoClient;
        _mongoDatabase = mongoDatabase;
    }

    /// <summary>
    ///     Represents the unique identifier for a transaction within the session.
    /// </summary>
    /// <remarks>
    ///     This property is assigned when a transaction starts and corresponds
    ///     to the server session identifier.
    /// </remarks>
    /// <value>
    ///     A <see cref="BsonDocument" /> representing the transaction identifier.
    ///     This value will be null if no transaction has been started.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an attempt is made to access the property before a transaction
    ///     is initiated.
    /// </exception>
    public BsonDocument? TransactionId { get; private set; }

    /// <summary>
    ///     Begins a new transaction asynchronously in the MongoDB session with optional session and transaction settings.
    /// </summary>
    /// <param name="clientSessionOptions">Optional settings for configuring the client session.</param>
    /// <param name="transactionOptions">Optional settings for configuring the transaction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the provided cancellation token.</exception>
    /// <exception cref="MongoException">Thrown when an error occurs while starting the session or transaction.</exception>
    public async Task BeginTransactionAsync(
        ClientSessionOptions? clientSessionOptions = null,
        TransactionOptions? transactionOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        _clientSessionHandle =
            await _mongoClient.StartSessionAsync(clientSessionOptions, cancellationToken);
        TransactionId = _clientSessionHandle.ServerSession.Id;
        _clientSessionHandle.StartTransaction(transactionOptions);
    }

    /// <summary>
    ///     Commits an ongoing MongoDB transaction.
    ///     Ensures that all operations within the transaction are successfully persisted. If the transaction
    ///     is not started or an error occurs during the commit, it attempts to roll back the transaction.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. If a cancellation is requested, an exception is thrown.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the transaction is not started before attempting to commit.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown if the commit operation fails for any other reasons.
    /// </exception>
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

    /// <summary>
    ///     Rolls back the current transaction, undoing any changes made during the transaction.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. The operation will throw an exception if cancellation is requested.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous rollback operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no transaction has been started before attempting to roll back.
    /// </exception>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_clientSessionHandle is null)
            throw new InvalidOperationException(
                "The transaction could not be rollback due to it is not started."
            );

        await _clientSessionHandle.AbortTransactionAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously creates a document in the specified collection within the MongoDB database.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be created. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the collection where the document will be inserted.</param>
    /// <param name="document">The document to be inserted into the collection.</param>
    /// <param name="cancellationToken">
    ///     An optional token to monitor cancellation requests for the operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
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

    /// <summary>
    ///     Asynchronously creates a document of the specified type in the MongoDB collection.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be created. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="document">
    ///     The document to be created. Cannot be null.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while awaiting the task. Optional.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the provided <paramref name="cancellationToken" />.
    /// </exception>
    public async Task CreateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(document, nameof(document));

        await CreateAsync(typeof(TDocument).Name, document, cancellationToken);
    }

    /// <summary>
    ///     Updates an existing document in the specified collection.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document that implements <see cref="IIdentifiableDocument" />.</typeparam>
    /// <param name="collectionName">The name of the MongoDB collection where the document is stored.</param>
    /// <param name="document">The document to be updated. It must not be null and must have a valid identifier.</param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Optional, defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
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

    /// <summary>
    ///     Updates an existing document of the specified type in the database.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document to update. Must implement <see cref="IIdentifiableDocument" />.</typeparam>
    /// <param name="document">The document to be updated. This must not be null.</param>
    /// <param name="cancellationToken">
    ///     A token to cancel the operation, if needed. Defaults to <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A task that represents the completion of the update operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(document, nameof(document));

        await UpdateAsync(typeof(TDocument).Name, document, cancellationToken);
    }

    /// <summary>
    ///     Deletes a document of the specified type from the MongoDB collection using its unique identifier.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be deleted, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection containing the document.</param>
    /// <param name="id">The unique identifier of the document to be deleted.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="id" /> is null.
    /// </exception>
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

    /// <summary>
    ///     Deletes a document of the specified type from the database using its unique identifier.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document being deleted. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">
    ///     The unique identifier of the document to delete.
    /// </param>
    /// <param name="cancellationToken">
    ///     Optional. A cancellation token to observe while waiting for the delete operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous delete operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="id" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task DeleteAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(id, nameof(id));

        await DeleteAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    /// <summary>
    ///     Deletes a document from the specified MongoDB collection.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be deleted. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection.</param>
    /// <param name="id">The identifier of the document to delete, represented as a <see cref="string" />.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="id" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled.
    /// </exception>
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

    /// <summary>
    ///     Asynchronously deletes a document of the specified type from the MongoDB collection by its ObjectId.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to delete, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">The ObjectId of the document to delete.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while waiting for the task to complete. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="id" /> is the default value.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public async Task DeleteAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        await DeleteAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    /// <summary>
    ///     Retrieves a document of a specified type from a MongoDB collection by its identifier.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to retrieve, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection to query.</param>
    /// <param name="id">The identifier of the document to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the document
    ///     of the specified type if found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="id" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via <paramref name="cancellationToken" />.
    /// </exception>
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

    /// <summary>
    ///     Asynchronously retrieves a document of type <typeparamref name="TDocument" /> by its unique identifier.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to retrieve, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">The unique identifier of the document to retrieve.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the document of type <typeparamref name="TDocument" /> matching the specified identifier.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="id" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<TDocument> GetByIdAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(id, nameof(id));

        return await GetByIdAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    /// <summary>
    ///     Retrieves a document of the specified type by its unique identifier from a MongoDB collection.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to retrieve. The type must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection where the document resides.</param>
    /// <param name="id">The unique identifier of the document as an <see cref="ObjectId" />.</param>
    /// <param name="cancellationToken">
    ///     An optional token to observe while waiting for the task to complete. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     The document, if found; otherwise, <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="id" /> is the default value of <see cref="ObjectId" />.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled.
    /// </exception>
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

    /// <summary>
    ///     Asynchronously retrieves a document of the specified type by its unique identifier.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to retrieve, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">
    ///     The unique identifier of the document to retrieve. This cannot be the default value of <see cref="ObjectId" />.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     An instance of <typeparamref name="TDocument" /> representing the document with the specified identifier.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="id" /> is the default value of <see cref="ObjectId" />.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" /> before it completes.
    /// </exception>
    public async Task<TDocument> GetByIdAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        return await GetByIdAsync<TDocument>(typeof(TDocument).Name, id, cancellationToken);
    }

    /// <summary>
    ///     Executes a query against the specified MongoDB collection and retrieves a list of documents
    ///     matching the provided filter criteria.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document being queried, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection to query.</param>
    /// <param name="filter">
    ///     An optional filter expression to apply to the query. If no filter is provided, all documents
    ///     in the collection will be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while waiting for the task to complete. If cancellation is requested,
    ///     the operation will terminate.
    /// </param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of matching documents.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collectionName" /> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public async Task<IList<TDocument>> QueryAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = null,
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

    /// <summary>
    ///     Executes an asynchronous query on a MongoDB collection and returns a list of documents that satisfy the specified
    ///     filter criteria.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to query, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="filter">
    ///     An optional LINQ expression specifying the filtering conditions for the query.
    ///     If not provided, all documents in the collection will be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while waiting for the task to complete.
    ///     If the operation is canceled, a <see cref="TaskCanceledException" /> will be thrown.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of documents matching the
    ///     query filter.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the provided cancellation token.</exception>
    public async Task<IList<TDocument>> QueryAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    )
        where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await QueryAsync(typeof(TDocument).Name, filter, cancellationToken);
    }

    /// <summary>
    ///     Counts the number of documents in a MongoDB collection based on the specified filter criteria.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be counted, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the collection to query.</param>
    /// <param name="filter">
    ///     An optional expression to filter the documents in the collection. Defaults to null, which means no
    ///     filtering is applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>The total number of documents in the collection that satisfy the specified filter criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="collectionName" /> is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<long> CountAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = null,
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

    /// <summary>
    ///     Counts the number of documents in a MongoDB collection that match the specified filter.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document to query. Must implement <see cref="IIdentifiableDocument" />.</typeparam>
    /// <param name="filter">
    ///     An optional filter expression to apply to the documents in the collection.
    ///     If null, all documents in the collection are counted.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     The total count of documents that satisfy the specified filter, or the total number of documents
    ///     if no filter is provided.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<long> CountAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await CountAsync(typeof(TDocument).Name, filter, cancellationToken);
    }
}