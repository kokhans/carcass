using System.Linq.Expressions;
using Carcass.Data.Core.Sessions.Abstracts;
using Carcass.Data.Firestore.Entities.Abstracts;

// ReSharper disable UnusedMember.Global

namespace Carcass.Data.Firestore.Sessions.Abstracts;

/// <summary>
///     Represents a session for interacting with Google Firestore, allowing for
///     CRUD operations and querying documents within specific collections.
/// </summary>
public interface IFirestoreSession : ISession
{
    /// <summary>
    ///     Creates a new document in the specified Firestore collection with the provided document data.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document being created. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">
    ///     The name of the Firestore collection where the document will be created.
    /// </param>
    /// <param name="document">
    ///     The document to be created in the specified collection.
    /// </param>
    /// <param name="cancellationToken">
    ///     An optional token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown if an error occurs during the creation process in Firestore.
    /// </exception>
    Task CreateAsync<TIdentifiableDocument>(
        string collectionName,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Creates a new document in the specified Firestore collection with the given identifier.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to be created, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">
    ///     The name of the Firestore collection where the document will be created.
    /// </param>
    /// <param name="id">
    ///     The unique identifier for the document within the collection.
    /// </param>
    /// <param name="document">
    ///     The document instance to create in the Firestore collection.
    /// </param>
    /// <param name="cancellationToken">
    ///     Optional. A token to cancel the operation before it completes.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" />, <paramref name="id" />, or <paramref name="document" /> is null.
    /// </exception>
    Task CreateAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Updates an existing document in the specified Firestore collection.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to be updated, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the Firestore collection containing the document to be updated.</param>
    /// <param name="document">
    ///     The document to update, which includes its new values and must implement
    ///     <see cref="IIdentifiableDocument" />.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the document does not have a valid identifier or if the update operation cannot be completed.
    /// </exception>
    Task UpdateAsync<TIdentifiableDocument>(
        string collectionName,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Deletes a document from a specified collection in Firestore using its unique identifier.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to be deleted. It must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the Firestore collection where the document resides.</param>
    /// <param name="id">The unique identifier of the document to delete.</param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None" /> if not provided.
    /// </param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="id" /> is null
    ///     or empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown if the deletion fails due to an invalid Firestore operation.</exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task DeleteAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Retrieves a document from a Firestore collection by its unique identifier.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to retrieve, implementing
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the Firestore collection.</param>
    /// <param name="id">The unique identifier of the document to retrieve.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>The document of type <typeparamref name="TIdentifiableDocument" /> if found; otherwise, throws an exception.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="id" /> is null
    ///     or empty.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown if no document with the specified <paramref name="id" /> was found in the
    ///     given <paramref name="collectionName" />.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task<TIdentifiableDocument> GetByIdAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    /// <summary>
    ///     Queries a Firestore collection and retrieves documents matching the specified filter.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to query, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">
    ///     The name of the Firestore collection to query.
    /// </param>
    /// <param name="filter">
    ///     An optional filter expression to apply on the documents in the collection.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token for canceling the asynchronous operation, with a default value.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a collection of documents
    ///     of the specified type that match the query parameters.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> is null or empty.
    /// </exception>
    Task<IEnumerable<TIdentifiableDocument>> QueryAsync<TIdentifiableDocument>(
        string collectionName,
        Expression<Func<TIdentifiableDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;
}