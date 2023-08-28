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

using System.Reflection;
using Carcass.Core;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Core.Helpers;
using Carcass.Core.Locators;
using Carcass.Data.Core.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Audit;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Data.EntityFrameworkCore.DbContexts.Abstracts;

public abstract class EntityFrameworkCoreDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    private readonly List<AuditEntry> _auditEntries = new();

    protected EntityFrameworkCoreDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected abstract Assembly Assembly { get; }

    public DbSet<AuditEntry> AuditEntries { get; set; }

    public override int SaveChanges()
    {
        AsyncHelper.RunSync(() => OnBeforeSaveChangesAsync());

        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AsyncHelper.RunSync(() => OnBeforeSaveChangesAsync());

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await OnBeforeSaveChangesAsync(cancellationToken);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AuditEntryConfiguration());
        modelBuilder.ApplyEntityConfigurations(Assembly);
        modelBuilder.ApplyIsDeletedQueryFilter();
    }

    private async Task OnBeforeSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DateTime dateTime = Clock.Current.UtcNow;

        IUserIdAccessor userIdAccessor = ServiceProviderLocator.Current.GetRequiredService<IUserIdAccessor>();
        string? userId = userIdAccessor.TryGetUserId();

        foreach (EntityEntry<IIdentifiableEntity> entityEntry in ChangeTracker.Entries<IIdentifiableEntity>())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    if (entityEntry.Entity is IAuditableEntity)
                        SetAuditBeforeAdd(entityEntry, userId, dateTime);
                    break;
                case EntityState.Modified:
                    if (entityEntry.Entity is IAuditableEntity)
                        SetAuditBeforeUpdate(entityEntry, userId, dateTime);
                    break;
                case EntityState.Deleted:
                    if (entityEntry.Entity is ISoftDeletableEntity)
                    {
                        entityEntry.State = EntityState.Modified;
                        entityEntry.CurrentValues[nameof(ISoftDeletable<Guid>.IsDeleted)] = true;
                    }

                    if (entityEntry.Entity is IAuditableEntity)
                        SetAuditBeforeDelete(entityEntry, userId, dateTime);
                    break;
            }

            AuditEntry? auditEntry = entityEntry.ToAuditEntry();
            if (auditEntry is not null)
                _auditEntries.Add(auditEntry);
        }

        await Set<AuditEntry>().AddRangeAsync(_auditEntries, cancellationToken);
        _auditEntries.Clear();
    }

    private static void SetAuditBeforeAdd(EntityEntry entityEntry, string? createdBy, DateTime createdAt)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        entityEntry.CurrentValues[nameof(IAuditableEntity.CreatedBy)] = createdBy;
        entityEntry.CurrentValues[nameof(IAuditableEntity.CreatedAt)] = createdAt;
    }

    private static void SetAuditBeforeUpdate(EntityEntry entityEntry, string? updatedBy, DateTime updatedAt)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        entityEntry.CurrentValues[nameof(IAuditableEntity.UpdatedAt)] = updatedAt;
        entityEntry.CurrentValues[nameof(IAuditableEntity.UpdatedBy)] = updatedBy;
    }

    private static void SetAuditBeforeDelete(EntityEntry entityEntry, string? deletedBy, DateTime deletedAt)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        SetAuditBeforeUpdate(entityEntry, deletedBy, deletedAt);
    }
}