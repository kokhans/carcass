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

using System.Linq.Expressions;
using Carcass.Core;
using Carcass.Data.Firestore.Entities.Abstracts;
using Carcass.Data.Firestore.Sessions.Abstracts;
using Google.Cloud.Firestore;

namespace Carcass.Data.Firestore.Sessions;

public sealed class FirestoreSession : IFirestoreSession
{
    private readonly FirestoreDb _firestoreDb;

    public FirestoreSession(FirestoreDb firestoreDb)
    {
        ArgumentVerifier.NotNull(firestoreDb, nameof(firestoreDb));

        _firestoreDb = firestoreDb;
    }

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

        await documentReference.SetAsync(document, SetOptions.Overwrite, cancellationToken: cancellationToken);
    }

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

    public async Task<IEnumerable<TIdentifiableDocument>> QueryAsync<TIdentifiableDocument>(
        string collectionName,
        Expression<Func<TIdentifiableDocument, bool>>? filter = default,
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