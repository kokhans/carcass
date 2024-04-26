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

using System.Data;
using Carcass.Core;
using Carcass.Data.EntityFrameworkCore.DbContexts.Abstracts;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Sessions.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Carcass.Data.EntityFrameworkCore.Sessions;

/// <summary>
///     Represents a session for interacting with Entity Framework Core, encapsulating database
///     context operations, transactions, and entity management functionalities.
/// </summary>
/// <typeparam name="TDbContext">
///     The type of the database context, constrained to inherit from
///     <see cref="EntityFrameworkCoreDbContext{TDbContext}" />.
/// </typeparam>
public sealed class EntityFrameworkCoreSession<TDbContext> : IEntityFrameworkCoreSession
    where TDbContext : EntityFrameworkCoreDbContext<TDbContext>
{
    /// <summary>
    ///     Represents the database context used to interact with the underlying data source.
    ///     This instance provides access to database operations and entity management for a specific DbContext.
    /// </summary>
    /// <remarks>
    ///     The database context is initialized during the session's instantiation and is used
    ///     for executing various operations such as transactions, queries, and CRUD operations.
    ///     Ensure the context is properly initialized and disposed of to manage resources effectively.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to access this instance if it has not been properly initialized.
    /// </exception>
    private readonly TDbContext? _dbContext;

    /// <summary>
    ///     Represents the current database transaction being managed by the session.
    ///     Used to control transactional operations such as begin, commit, and rollback within the
    ///     Entity Framework Core session context.
    /// </summary>
    /// <remarks>
    ///     This variable holds the instance of <see cref="Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction" />
    ///     managing the active database transaction. It is nullable and can be null if no transaction
    ///     has been initiated.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when operations like commit or rollback are attempted on a null transaction.
    /// </exception>
    private IDbContextTransaction? _dbContextTransaction;

    /// <summary>
    ///     Provides an implementation of the <see cref="IEntityFrameworkCoreSession" /> interface to manage
    ///     entity persistence using a specified Entity Framework Core DbContext.
    /// </summary>
    /// <typeparam name="TDbContext">
    ///     The type of the DbContext used by this session, constrained to inherit from
    ///     <see cref="EntityFrameworkCoreDbContext{TDbContext}" />.
    /// </typeparam>
    public EntityFrameworkCoreSession(TDbContext dbContext)
    {
        ArgumentVerifier.NotNull(dbContext, nameof(dbContext));

        _dbContext = dbContext;
    }

    /// <summary>
    ///     Represents the unique identifier of the active database transaction.
    /// </summary>
    /// <value>
    ///     A <see cref="Guid" /> that uniquely identifies the current transaction.
    ///     It is <see langword="null" /> if no transaction is currently active.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if there is an attempt to access the property when the corresponding <see cref="DbContext" /> is not
    ///     available.
    /// </exception>
    public Guid? TransactionId { get; private set; }

    /// <summary>
    ///     Begins a database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level to be used for the transaction.</param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete. Defaults to <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the transaction cannot be started because the database context is null.
    /// </exception>
    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"The transaction could not be started due to {nameof(DbContext)} is null."
            );

        _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        TransactionId = _dbContextTransaction.TransactionId;
    }

    /// <summary>
    ///     Commits the current database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the commit operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if there is no active transaction to commit.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the cancellation token.</exception>
    /// <exception cref="Exception">Thrown if the commit fails and rollback also encounters an issue.</exception>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (_dbContextTransaction is null)
                throw new InvalidOperationException("The transaction could not be committed due to it is not started.");

            await _dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    ///     Rolls back the current database transaction asynchronously if it has been started.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests, allowing the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an attempt is made to roll back a transaction that has not been started.
    /// </exception>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dbContextTransaction is null)
            throw new InvalidOperationException("The transaction could not be rollback due to it is not started.");

        await _dbContextTransaction.RollbackAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously saves all changes made in the current session using the associated database context.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous save operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the database context is null.
    /// </exception>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dbContext is null)
            throw new InvalidOperationException($"The changes could not be saved due to {nameof(DbContext)} is null.");

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously creates a new entity in the database context.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">
    ///     The type of the entity being created, which must implement <see cref="IIdentifiableEntity" />.
    /// </typeparam>
    /// <param name="entity">The entity instance to be added to the database context.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="entity" /> parameter is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the database context instance is null when attempting to add the entity.
    /// </exception>
    public async Task CreateAsync<TIdentifiableEntity>(
        TIdentifiableEntity entity,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(entity, nameof(entity));

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"Entity {entity.Id} could not be created due to {nameof(DbContext)} is null."
            );

        await _dbContext.Set<TIdentifiableEntity>().AddAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Updates the specified entity within the current context.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">The type of the entity implementing <see cref="IIdentifiableEntity" />.</typeparam>
    /// <param name="entity">The entity to be updated. Must not be null.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="entity" /> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the update operation cannot proceed due to the context being null.
    /// </exception>
    public Task UpdateAsync<TIdentifiableEntity>(
        TIdentifiableEntity entity,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(entity, nameof(entity));

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"Entity {entity.Id} could not be updated due to {nameof(DbContext)} is null.");

        _dbContext.Set<TIdentifiableEntity>().Update(entity);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Deletes an entity of the specified type identified by the given ID from the database asynchronously.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">
    ///     The type of the entity to be deleted. Must implement
    ///     <see cref="IIdentifiableEntity" />.
    /// </typeparam>
    /// <param name="id">The unique identifier of the entity to be deleted.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="id" /> has a default value.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the database context is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task DeleteAsync<TIdentifiableEntity>(
        Guid id,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        TIdentifiableEntity entity = await GetByIdAsync<TIdentifiableEntity>(id, true, cancellationToken);

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"Entity {entity.Id} could not be deleted due to {nameof(DbContext)} is null."
            );

        _dbContext.Set<TIdentifiableEntity>().Remove(entity);
    }

    /// <summary>
    ///     Attempts to retrieve an entity of the specified type by its unique identifier.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">The type of the entity, which must implement <see cref="IIdentifiableEntity" />.</typeparam>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="asNoTracking">
    ///     Whether to retrieve the entity without tracking it in the DbContext.
    ///     Set to true to disable change tracking.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     The entity matching the specified identifier, or null if no such entity is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="id" /> is the default value of <see cref="System.Guid" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the DbContext is null or if the entity could not be retrieved due to another invalid state.
    /// </exception>
    public async Task<TIdentifiableEntity?> TryGetByIdAsync<TIdentifiableEntity>(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"Entity {id} could not be retrieved due to {nameof(DbContext)} is null."
            );

        return asNoTracking == false
            ? await _dbContext
                .Set<TIdentifiableEntity>()
                .SingleOrDefaultAsync(e => e.Id == id, cancellationToken)
            : await _dbContext
                .Set<TIdentifiableEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <summary>
    ///     Attempts to asynchronously retrieve an entity by its unique identifier and tenant identifier from the database.
    /// </summary>
    /// <typeparam name="TTenantifiableEntity">
    ///     The type of the entity to retrieve, which must implement <see cref="ITenantifiableEntity" />.
    /// </typeparam>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="tenantId">The identifier of the tenant associated with the entity.</param>
    /// <param name="asNoTracking">
    ///     A flag indicating whether to retrieve the entity without tracking changes in the context.
    ///     Defaults to <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> that can be used to cancel the operation.
    ///     Defaults to <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     The entity of type <typeparamref name="TTenantifiableEntity" /> if found; otherwise, <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the database context is not initialized or the operation fails.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the provided <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<TTenantifiableEntity?> TryGetByIdAsync<TTenantifiableEntity>(
        Guid id,
        string? tenantId,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    ) where TTenantifiableEntity : class, ITenantifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"Entity {id} could not be retrieved due to {nameof(DbContext)} is null."
            );

        return asNoTracking == false
            ? await _dbContext
                .Set<TTenantifiableEntity>()
                .SingleOrDefaultAsync(te => te.Id == id && te.TenantId == tenantId, cancellationToken)
            : await _dbContext
                .Set<TTenantifiableEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(te => te.Id == id && te.TenantId == tenantId, cancellationToken);
    }

    /// <summary>
    ///     Retrieves an entity of type <typeparamref name="TIdentifiableEntity" /> from the database by its unique identifier.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">
    ///     The type of the entity to retrieve, which must implement
    ///     <see cref="IIdentifiableEntity" />.
    /// </typeparam>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="asNoTracking">
    ///     Specifies whether the entity should be tracked by the change tracker. Set to true for no
    ///     tracking.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The entity of type <typeparamref name="TIdentifiableEntity" /> if found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the entity with the specified <paramref name="id" /> is not found.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<TIdentifiableEntity> GetByIdAsync<TIdentifiableEntity>(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        TIdentifiableEntity? entity = await TryGetByIdAsync<TIdentifiableEntity>(id, asNoTracking, cancellationToken);

        return entity ?? throw new InvalidOperationException($"Entity {id} not found.");
    }

    /// <summary>
    ///     Retrieves an entity of type <typeparamref name="TTenantifiableEntity" /> by its unique identifier and tenant
    ///     identifier.
    ///     Ensures the entity exists; otherwise, an exception is thrown.
    /// </summary>
    /// <typeparam name="TTenantifiableEntity">
    ///     The type of the entity to retrieve, which must implement <see cref="ITenantifiableEntity" />.
    /// </typeparam>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="tenantId">The tenant identifier associated with the entity. This can be null if not applicable.</param>
    /// <param name="asNoTracking">
    ///     Specifies whether the entity should be retrieved without tracking by the Entity Framework Change Tracker.
    ///     Default is false.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The entity of type <typeparamref name="TTenantifiableEntity" /> corresponding to the specified
    ///     <paramref name="id" />.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="id" /> is a default value.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no entity is found for the given <paramref name="id" /> and <paramref name="tenantId" /> combination.
    /// </exception>
    public async Task<TTenantifiableEntity> GetByIdAsync<TTenantifiableEntity>(
        Guid id,
        string? tenantId,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    ) where TTenantifiableEntity : class, ITenantifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        TTenantifiableEntity? entity =
            await TryGetByIdAsync<TTenantifiableEntity>(id, tenantId, asNoTracking, cancellationToken);

        return entity ?? throw new InvalidOperationException($"Entity {id} not found.");
    }

    /// <summary>
    ///     Retrieves a queryable collection of entities of the specified type.
    ///     Allows for configurable tracking behavior and cancellation support.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">
    ///     The type of the entity to query. Must implement <see cref="IIdentifiableEntity" />.
    /// </typeparam>
    /// <param name="asNoTracking">
    ///     Indicates whether the query should avoid tracking entities.
    ///     Defaults to false, meaning entities are tracked by the context.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     An <see cref="IQueryable{TIdentifiableEntity}" /> representing the requested entities.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the underlying DbContext is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the cancellation token is canceled.
    /// </exception>
    public IQueryable<TIdentifiableEntity> Query<TIdentifiableEntity>(
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dbContext is null)
            throw new InvalidOperationException($"Entities could not be retrieved due to {nameof(DbContext)} is null.");

        return asNoTracking == false
            ? _dbContext.Set<TIdentifiableEntity>()
            : _dbContext.Set<TIdentifiableEntity>().AsNoTracking();
    }

    /// <summary>
    ///     Retrieves a queryable collection of entities implementing <see cref="ITenantifiableEntity" /> for a specific
    ///     tenant.
    ///     Allows optional tracking or no-tracking query modes.
    /// </summary>
    /// <typeparam name="TTenantifiableEntity">
    ///     The type of entity to query. Must implement <see cref="ITenantifiableEntity" />.
    /// </typeparam>
    /// <param name="tenantId">The tenant identifier to filter the entities.</param>
    /// <param name="asNoTracking">Specifies if the query should be executed in no-tracking mode. Default is <c>false</c>.</param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Default is
    ///     <see cref="CancellationToken.None" />.
    /// </param>
    /// <returns>
    ///     An <see cref="IQueryable{T}" /> containing filtered results based on the <paramref name="tenantId" /> and tracking
    ///     mode.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the underlying <see cref="DbContext" /> is null, which prevents retrieval of entities.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public IQueryable<TTenantifiableEntity> Query<TTenantifiableEntity>(
        string? tenantId,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    ) where TTenantifiableEntity : class, ITenantifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dbContext is null)
            throw new InvalidOperationException($"Entities could not be retrieved due to {nameof(DbContext)} is null.");

        return asNoTracking == false
            ? _dbContext
                .Set<TTenantifiableEntity>()
                .Where(te => te.TenantId == tenantId)
            : _dbContext
                .Set<TTenantifiableEntity>()
                .Where(te => te.TenantId == tenantId)
                .AsNoTracking();
    }

    /// <summary>
    ///     Disposes resources used by the current instance of EntityFrameworkCoreSession,
    ///     including the underlying database context and transaction objects.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the method is called on an already disposed instance.
    /// </exception>
    public void Dispose()
    {
        _dbContextTransaction?.Dispose();
        _dbContext?.Dispose();
    }

    /// <summary>
    ///     Asynchronously disposes the session, including the database context and any associated transactions.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the session, database context, or transaction has already been
    ///     disposed.
    /// </exception>
    public async ValueTask DisposeAsync()
    {
        if (_dbContextTransaction is not null)
            await _dbContextTransaction.DisposeAsync();

        if (_dbContext is not null)
            await _dbContext.DisposeAsync();
    }
}