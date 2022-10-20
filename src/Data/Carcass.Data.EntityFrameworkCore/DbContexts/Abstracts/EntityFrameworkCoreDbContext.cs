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

using System.Reflection;
using Carcass.Core;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Core.Locators;
using Carcass.Data.Core.Commands.Notifications;
using Carcass.Data.Core.Commands.Notifications.Dispatchers.Abstracts;
using Carcass.Data.Core.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Commands.Notifications;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Data.EntityFrameworkCore.DbContexts.Abstracts;

public abstract class EntityFrameworkCoreDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    private readonly List<AuditTrailEntry> _auditTrailEntries;

    protected EntityFrameworkCoreDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
        _auditTrailEntries = new List<AuditTrailEntry>();
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected abstract Assembly Assembly { get; }

    public override int SaveChanges()
    {
        OnBeforeSaveChanges();

        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        OnBeforeSaveChanges();
        int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await OnAfterSaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyEntityConfigurations(Assembly);
        modelBuilder.ApplyIsDeletedQueryFilter();
    }

    private void OnBeforeSaveChanges()
    {
        DateTime dateTime = Clock.Current.UtcNow;
        IUserIdAccessor userIdAccessor = ServiceProviderLocator.Current.GetRequiredService<IUserIdAccessor>();
        string? userId = userIdAccessor.TryGetUserId();

        foreach (EntityEntry<IIdentifiableEntity> entityEntry in ChangeTracker.Entries<IIdentifiableEntity>())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    {
                        if (entityEntry.Entity is IAuditableEntity)
                        {
                            SetAuditBeforeAdd(entityEntry, userId, dateTime);
                            _auditTrailEntries.Add(new AuditTrailEntry(entityEntry));
                        }

                        break;
                    }
                case EntityState.Modified:
                    {
                        if (entityEntry.Entity is IAuditableEntity)
                        {
                            SetAuditBeforeUpdate(entityEntry, userId, dateTime);
                            _auditTrailEntries.Add(new AuditTrailEntry(entityEntry));
                        }

                        break;
                    }
                case EntityState.Deleted:
                    switch (entityEntry.Entity)
                    {
                        case ISoftDeletableEntity:
                            entityEntry.State = EntityState.Modified;
                            entityEntry.CurrentValues[nameof(ISoftDeletable<Guid>.IsDeleted)] = true;
                            break;
                        case IAuditableEntity:
                            SetAuditBeforeDelete(entityEntry, userId, dateTime);
                            _auditTrailEntries.Add(new AuditTrailEntry(entityEntry));
                            break;
                    }

                    break;
            }
        }
    }

    private async Task OnAfterSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_auditTrailEntries.Any())
        {
            List<AuditTrailTransactionalNotification> notifications =
                _auditTrailEntries.Select(ate => ate.ToAuditTrailTransactionalDomainEvent()).ToList();
            List<INotificationDispatcher> notificationDispatchers =
                ServiceProviderLocator.Current.GetRequiredServices<INotificationDispatcher>().ToList();

            foreach (INotificationDispatcher notificationDispatcher in notificationDispatchers)
                foreach (AuditTrailTransactionalNotification notification in notifications)
                    await notificationDispatcher.DispatchNotificationAsync(notification, cancellationToken);
        }
    }

    private static void SetAuditBeforeAdd(EntityEntry entityEntry, string? createdBy, DateTime createdAt)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        entityEntry.CurrentValues[nameof(IAuditable<Guid>.CreatedBy)] = createdBy;
        entityEntry.CurrentValues[nameof(IAuditable<Guid>.CreatedAt)] = createdAt;
    }

    private static void SetAuditBeforeUpdate(EntityEntry entityEntry, string? updatedBy, DateTime updatedAt)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        entityEntry.CurrentValues[nameof(IAuditable<Guid>.UpdatedAt)] = updatedAt;
        entityEntry.CurrentValues[nameof(IAuditable<Guid>.UpdatedBy)] = updatedBy;
    }

    private static void SetAuditBeforeDelete(EntityEntry entityEntry, string? deletedBy, DateTime deletedAt)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        SetAuditBeforeUpdate(entityEntry, deletedBy, deletedAt);
    }
}