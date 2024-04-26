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

using System.Linq.Expressions;
using Carcass.Data.Core.Sessions.Abstracts;
using Carcass.Data.MongoDb.Entities.Abstracts;
using MongoDB.Bson;
using MongoDB.Driver;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Carcass.Data.MongoDb.Sessions.Abstracts;

/// <summary>
///     Defines a session for interacting with MongoDB, including support for transactions,
///     CRUD operations, and querying documents.
/// </summary>
public interface IMongoDbSession : ITransactionalSession<BsonDocument>
{
    /// <summary>
    ///     Initiates a new transaction for the current MongoDB session.
    /// </summary>
    /// <param name="clientSessionOptions">Optional client session configuration options.</param>
    /// <param name="transactionOptions">Optional transaction configuration options such as read and write concerns.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="MongoException">Thrown when there is an error starting the transaction.</exception>
    Task BeginTransactionAsync(
        ClientSessionOptions? clientSessionOptions = null,
        TransactionOptions? transactionOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Creates a new document in the specified collection asynchronously.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be created. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection where the document will be created.</param>
    /// <param name="document">The document to create.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="collectionName" /> or
    ///     <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="MongoException">Thrown if an error occurs when communicating with the MongoDB server.</exception>
    Task CreateAsync<TDocument>(
        string collectionName,
        TDocument document,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Inserts a document into a MongoDB collection.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be created, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="document">
    ///     The document to insert into the MongoDB collection.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete. Defaults to a non-cancelable token.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous create operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="MongoException">
    ///     Thrown when an error occurs during the operation with MongoDB.
    /// </exception>
    Task CreateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Updates an existing document in the specified collection within the MongoDB session.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be updated, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection where the document is stored.</param>
    /// <param name="document">
    ///     The document to be updated. The document must include a valid identifier to locate the existing
    ///     record.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to cancel the
    ///     operation.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="document" />
    ///     is null.
    /// </exception>
    /// <exception cref="MongoException">Thrown when an error occurs during the update operation.</exception>
    Task UpdateAsync<TDocument>(
        string collectionName,
        TDocument document,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Updates an existing document in the database.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be updated. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="document">The document to be updated in the database. Must not be null.</param>
    /// <param name="cancellationToken">
    ///     A token to observe while awaiting the update operation. Defaults to
    ///     <see cref="CancellationToken.None" /> if not provided.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the document is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the operation cannot be completed due to invalid state or
    ///     configuration.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task UpdateAsync<TDocument>(TDocument document, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Deletes a document from the specified collection in the MongoDB database.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document to delete.</typeparam>
    /// <param name="collectionName">The name of the collection containing the document.</param>
    /// <param name="id">The string representation of the unique identifier of the document to delete.</param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. The operation will be cancelled if the token is triggered.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="id" /> is null or empty.
    /// </exception>
    /// <exception cref="MongoException">
    ///     Thrown if an error occurs while deleting the document from the MongoDB database.
    /// </exception>
    Task DeleteAsync<TDocument>(string collectionName, string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Deletes a document of the specified type from the MongoDB collection using its string identifier.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be deleted. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">The string identifier of the document to be deleted.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="id" /> is null or empty.</exception>
    Task DeleteAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Deletes a document identified by the specified <paramref name="id" /> from the collection with the specified
    ///     <paramref name="collectionName" />.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to delete, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection from which the document will be deleted.</param>
    /// <param name="id">The unique identifier of the document to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token to observe while awaiting the task.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collectionName" /> is null or empty.</exception>
    /// <exception cref="MongoException">Thrown if there is an error during the delete operation.</exception>
    Task DeleteAsync<TDocument>(string collectionName, ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Deletes a document from the collection by its ObjectId.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be deleted. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">
    ///     The unique ObjectId of the document to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests during the operation.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the provided id is null.
    /// </exception>
    /// <exception cref="MongoException">
    ///     Thrown when there is an error communicating with the MongoDB server.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the provided cancellation token.
    /// </exception>
    Task DeleteAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Retrieves a document of the specified type from the database based on the provided string ID.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document being retrieved, which must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the collection where the document is stored.</param>
    /// <param name="id">The string value of the document's unique identifier.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <returns>The requested document of type <typeparamref name="TDocument" /> if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="id" /> is
    ///     null or empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown if the document is not found in the specified collection.</exception>
    /// <exception cref="MongoException">Thrown if an error occurs while communicating with the MongoDB database.</exception>
    Task<TDocument> GetByIdAsync<TDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Retrieves a document of the specified type by its identifier from the database.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document to retrieve. Must implement IIdentifiableDocument.</typeparam>
    /// <param name="id">The identifier of the document to be retrieved.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The document of the specified type if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="id" /> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the document could not be retrieved due to an invalid state.</exception>
    /// <exception cref="MongoException">Thrown if a database error occurs during the operation.</exception>
    Task<TDocument> GetByIdAsync<TDocument>(string id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Retrieves a document of the specified type by its identifier from the given collection.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document to be retrieved.</typeparam>
    /// <param name="collectionName">The name of the MongoDB collection to search in.</param>
    /// <param name="id">The unique identifier of the document to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete. Optional.</param>
    /// <returns>The document of the specified type if found, otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collectionName" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="collectionName" /> is empty or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the specified document type or identifier is invalid.</exception>
    /// <exception cref="MongoException">Thrown if there is an error during the MongoDB operation.</exception>
    Task<TDocument> GetByIdAsync<TDocument>(
        string collectionName,
        ObjectId id,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Retrieves a document by its unique ObjectId from the MongoDB collection.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to retrieve; must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="id">The ObjectId of the document to retrieve.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to signal the operation should be canceled.
    /// </param>
    /// <returns>
    ///     The document of the specified type if found, or null if no document matches the given ObjectId.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the id parameter is invalid.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if multiple documents with the same id are found.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown if the operation is canceled via the provided cancellation token.
    /// </exception>
    /// <exception cref="MongoException">
    ///     Thrown if an error occurs during the retrieval operation.
    /// </exception>
    Task<TDocument> GetByIdAsync<TDocument>(ObjectId id, CancellationToken cancellationToken = default)
        where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Queries documents from a MongoDB collection asynchronously based on the specified filter.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document to query, which implements <see cref="IIdentifiableDocument" />.</typeparam>
    /// <param name="collectionName">The name of the MongoDB collection to query.</param>
    /// <param name="filter">
    ///     An optional expression used to filter the documents in the collection. If null, no filter is applied, and all
    ///     documents will be returned.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of documents of type
    ///     <typeparamref name="TDocument" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> is null or empty.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="MongoException">
    ///     Thrown if an error occurs while querying the MongoDB collection.
    /// </exception>
    Task<IList<TDocument>> QueryAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Queries the database for documents of the specified type that match the provided filter.
    /// </summary>
    /// <typeparam name="TDocument">The type of the documents to query. Must implement <see cref="IIdentifiableDocument" />.</typeparam>
    /// <param name="filter">
    ///     An optional expression to filter the documents. If not provided, all documents of the specified type will be
    ///     retrieved.
    /// </param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of documents of type
    ///     <typeparamref name="TDocument" />
    ///     that match the specified filter.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="MongoException">
    ///     Thrown when an error occurs while querying the database.
    /// </exception>
    Task<IList<TDocument>> QueryAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Counts the number of documents in the specified collection that match the given filter criteria.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document being counted. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the MongoDB collection to query.</param>
    /// <param name="filter">
    ///     An optional filter expression to apply to the documents. If null, all documents in the collection
    ///     are counted.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The count of documents matching the specified filter criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collectionName" /> is null or empty.</exception>
    /// <exception cref="MongoException">Thrown for errors that occur during the count operation in MongoDB.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task<long> CountAsync<TDocument>(
        string collectionName,
        Expression<Func<TDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Asynchronously counts the number of documents in the collection that match the specified filter.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     The type of the document to be counted. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="filter">
    ///     A filter expression to define the criteria for counting documents. Pass null to count all
    ///     documents.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>Returns the number of documents in the collection matching the provided filter.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the document type is null.</exception>
    /// <exception cref="MongoException">Thrown when there is an error during the operation in the database.</exception>
    Task<long> CountAsync<TDocument>(
        Expression<Func<TDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TDocument : class, IIdentifiableDocument;
}