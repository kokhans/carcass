using Carcass.Data.Core.Sessions.Abstracts;
using Carcass.Data.Firestore.Entities.Abstracts;
using System.Linq.Expressions;

namespace Carcass.Data.Firestore.Sessions.Abstracts;

public interface IFirestoreSession : ISession
{
    Task CreateAsync<TIdentifiableDocument>(
        string collectionName,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    Task CreateAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    Task UpdateAsync<TIdentifiableDocument>(
        string collectionName,
        TIdentifiableDocument document,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    Task DeleteAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    Task<TIdentifiableDocument> GetByIdAsync<TIdentifiableDocument>(
        string collectionName,
        string id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;

    Task<IEnumerable<TIdentifiableDocument>> QueryAsync<TIdentifiableDocument>(
        string collectionName,
        Expression<Func<TIdentifiableDocument, bool>>? filter = default,
        CancellationToken cancellationToken = default
    ) where TIdentifiableDocument : class, IIdentifiableDocument;
}