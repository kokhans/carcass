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

using System.Data;
using Carcass.Core;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Sessions.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Carcass.Data.EntityFrameworkCore.Sessions;

public sealed class EntityFrameworkCoreSession<TDbContext> : IEntityFrameworkCoreSession
    where TDbContext : DbContext
{
    private readonly TDbContext? _dbContext;
    private IDbContextTransaction? _dbContextTransaction;

    public EntityFrameworkCoreSession(TDbContext dbContext)
    {
        ArgumentVerifier.NotNull(dbContext, nameof(dbContext));

        _dbContext = dbContext;
    }

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

    public Guid TransactionId { get; private set; }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (_dbContext is null || _dbContextTransaction is null)
                throw new InvalidOperationException("The transaction could not be committed due to it is not started.");

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContextTransaction.CommitAsync(cancellationToken);
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

        if (_dbContextTransaction is null)
            throw new InvalidOperationException("The transaction could not be rollback due to it is not started.");

        await _dbContextTransaction.RollbackAsync(cancellationToken);
    }

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

    public async Task<TIdentifiableEntity?> TryGetByIdAsync<TIdentifiableEntity>(
        Guid id,
        bool asNoTracking = default,
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

    public async Task<TIdentifiableEntity> GetByIdAsync<TIdentifiableEntity>(
        Guid id,
        bool asNoTracking = default,
        CancellationToken cancellationToken = default
    ) where TIdentifiableEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(id, nameof(id));

        TIdentifiableEntity? entity = await TryGetByIdAsync<TIdentifiableEntity>(id, asNoTracking, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException($"Entity {id} not found.");

        return entity;
    }

    public IQueryable<TEntity> Query<TEntity>(
        bool asNoTracking = default,
        CancellationToken cancellationToken = default
    ) where TEntity : class, IIdentifiableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dbContext is null)
            throw new InvalidOperationException(
                $"Entities could not be retrieved due to {nameof(DbContext)} is null."
            );

        return asNoTracking == false
            ? _dbContext.Set<TEntity>()
            : _dbContext.Set<TEntity>().AsNoTracking();
    }

    public void Dispose()
    {
        _dbContextTransaction?.Dispose();
        _dbContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContextTransaction is not null)
            await _dbContextTransaction.DisposeAsync();

        if (_dbContext is not null)
            await _dbContext.DisposeAsync();
    }
}