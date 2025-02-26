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
using Carcass.Core;
using Carcass.Data.Firestore.Entities.Abstracts;
using Carcass.Data.Firestore.Sessions.Abstracts;
using Google.Cloud.Firestore;

namespace Carcass.Data.Firestore.Sessions;

/// <summary>
///     Provides functionality for managing Firestore database sessions,
///     allowing operations such as create, update, delete, and query on
///     Firestore collections and documents.
/// </summary>
public sealed class FirestoreSession : IFirestoreSession
{
    /// <summary>
    ///     Represents the underlying Firestore database instance used for interacting with Firestore collections and
    ///     documents.
    ///     Provides functionality for executing CRUD operations within the Firestore database.
    /// </summary>
    /// <remarks>
    ///     This variable is initialized through dependency injection and is essential for all Firestore operations
    ///     handled by the <see cref="FirestoreSession" /> class.
    /// </remarks>
    /// <type>FirestoreDb</type>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown during initialization if the provided FirestoreDb instance is null.
    /// </exception>
    private readonly FirestoreDb _firestoreDb;

    /// <summary>
    ///     Represents a Firestore session for interacting with Firestore databases.
    ///     Provides methods for performing CRUD operations, querying collections,
    ///     and managing data within Firestore.
    /// </summary>
    public FirestoreSession(FirestoreDb firestoreDb)
    {
        ArgumentVerifier.NotNull(firestoreDb, nameof(firestoreDb));

        _firestoreDb = firestoreDb;
    }

    /// <summary>
    ///     Creates a new document in the specified Firestore collection with a generated ID.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to be created. Must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">
    ///     The name of the Firestore collection where the document will be created. Must not be null or empty.
    /// </param>
    /// <param name="document">
    ///     The document to add to the collection. Must not be null.
    /// </param>
    /// <param name="cancellationToken">
    ///     Optional token to observe while waiting for the operation to complete. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="document" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    public async Task CreateAsync<TIdentifiableDocument>(
        string collectionName,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(document, nameof(document));

        CollectionReference collectionReference = _firestoreDb.Collection(collectionName);
        await collectionReference.AddAsync(document, cancellationToken);
    }

    /// <summary>
    ///     Creates a document in the specified Firestore collection with the given identifier.
    ///     If a document with the specified identifier already exists, it will be overwritten.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to create, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the Firestore collection where the document should be created.</param>
    /// <param name="id">The unique identifier for the document.</param>
    /// <param name="document">The document to create in the collection.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while waiting for the task to complete. Defaults to
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A task that represents the asynchronous operation of creating the document.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" />, <paramref name="id" />, or <paramref name="document" /> is
    ///     <c>null</c>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    public async Task CreateAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(id, nameof(id));
        ArgumentVerifier.NotNull(document, nameof(document));

        DocumentReference documentReference = _firestoreDb.Collection(collectionName).Document(id);

        await documentReference.SetAsync(document, SetOptions.Overwrite, cancellationToken);
    }

    /// <summary>
    ///     Updates an existing document in the specified Firestore collection. If the document exists, only the specified
    ///     fields are merged;
    ///     other fields remain unchanged.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to update, which must implement the
    ///     <see cref="IIdentifiableDocument" /> interface.
    /// </typeparam>
    /// <param name="collectionName">The name of the Firestore collection containing the document to update.</param>
    /// <param name="document">The document to update, including its identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="document" />
    ///     is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task UpdateAsync<TIdentifiableDocument>(
        string collectionName,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(document, nameof(document));

        DocumentReference documentReference = _firestoreDb.Collection(collectionName).Document(document.Id.Id);
        await documentReference.SetAsync(document, SetOptions.MergeAll, cancellationToken);
    }

    /// <summary>
    ///     Deletes a document with the specified ID from the Firestore collection.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to delete, which must implement <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">
    ///     The name of the Firestore collection containing the document.
    /// </param>
    /// <param name="id">
    ///     The unique identifier of the document to delete.
    /// </param>
    /// <param name="cancellationToken">
    ///     Optional. A cancellation token that can be used to cancel the delete operation.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous delete operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="collectionName" /> or <paramref name="id" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task DeleteAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(id, nameof(id));

        DocumentReference documentReference = _firestoreDb.Collection(collectionName).Document(id);
        await documentReference.DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    ///     Retrieves a document from the specified collection by its ID asynchronously.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of the document to retrieve that implements <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">
    ///     The name of the Firestore collection containing the document.
    /// </param>
    /// <param name="id">
    ///     The unique identifier of the document to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The retrieved document of type <typeparamref name="TIdentifiableDocument" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="collectionName" /> or <paramref name="id" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<TIdentifiableDocument> GetByIdAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));
        ArgumentVerifier.NotNull(id, nameof(id));

        DocumentReference documentReference = _firestoreDb.Collection(collectionName).Document(id);
        DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync(cancellationToken);

        return documentSnapshot.ConvertTo<TIdentifiableDocument>();
    }

    /// <summary>
    ///     Retrieves a collection of documents from the specified Firestore collection based on an optional filter expression.
    /// </summary>
    /// <typeparam name="TIdentifiableDocument">
    ///     The type of documents in the Firestore collection implementing
    ///     <see cref="IIdentifiableDocument" />.
    /// </typeparam>
    /// <param name="collectionName">The name of the Firestore collection to query.</param>
    /// <param name="filter">An optional filter expression to apply to the documents.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An enumerable of documents matching the specified type and filter, or all documents if no filter is applied.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="collectionName" /> is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<IEnumerable<TIdentifiableDocument>> QueryAsync<TIdentifiableDocument>(
        string collectionName,
        Expression<Func<TIdentifiableDocument, bool>>? filter = null,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(collectionName, nameof(collectionName));

        CollectionReference collectionReference = _firestoreDb.Collection(collectionName);
        QuerySnapshot querySnapshot = await collectionReference.GetSnapshotAsync(cancellationToken);
        IEnumerable<TIdentifiableDocument> documents = querySnapshot.Documents
            .Select(documentSnapshot => documentSnapshot.ConvertTo<TIdentifiableDocument>());

        return filter != null ? documents.Where(filter.Compile()) : documents;
    }
}