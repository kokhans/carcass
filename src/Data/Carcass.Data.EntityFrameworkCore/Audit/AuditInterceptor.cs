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

using Carcass.Core;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Data.Core.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Carcass.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Carcass.Data.EntityFrameworkCore.Audit;

/// <summary>
///     Intercepts Entity Framework Core save changes operations to handle audit-related functionality.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    ///     Represents an instance of the <see cref="AuditOptionsExtension" /> class,
    ///     used to configure audit-specific behaviors in the database context.
    /// </summary>
    private readonly AuditOptionsExtension _auditOptionsExtension;

    /// <summary>
    ///     Provides the current date and time information, allowing the retrieval of
    ///     current UTC date and time to support auditing operations.
    /// </summary>
    /// <remarks>
    ///     This variable is used to ensure consistent and accurate time values are supplied
    ///     for audit-related operations, such as logging timestamps during entity changes.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown during initialization if the <see cref="TimeProvider" /> instance is null.
    /// </exception>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    ///     Provides access to the user ID associated with the current context.
    /// </summary>
    /// <remarks>
    ///     This field is used to fetch the user ID via a user ID accessor implementation,
    ///     typically to associate user-related information with operations such as auditing changes.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown during initialization if the provided <see cref="IUserIdAccessor" /> instance is null.
    /// </exception>
    private readonly IUserIdAccessor _userIdAccessor;

    /// <summary>
    ///     Intercepts SaveChanges operations to apply auditing functionality
    ///     for tracking changes in the database entities.
    /// </summary>
    public AuditInterceptor(
        TimeProvider timeProvider,
        IUserIdAccessor userIdAccessor,
        AuditOptionsExtension auditOptionsExtension)
    {
        ArgumentVerifier.NotNull(timeProvider, nameof(timeProvider));
        ArgumentVerifier.NotNull(userIdAccessor, nameof(userIdAccessor));
        ArgumentVerifier.NotNull(auditOptionsExtension, nameof(auditOptionsExtension));

        _timeProvider = timeProvider;
        _userIdAccessor = userIdAccessor;
        _auditOptionsExtension = auditOptionsExtension;
    }

    /// <summary>
    ///     Intercepts the asynchronous save changes operation in the DbContext and applies audit logic.
    ///     This method handles adding, updating, and soft-deleting entities based on their state,
    ///     while also managing audit entries if configured.
    /// </summary>
    /// <param name="eventData">Contextual event data for the save changes operation.</param>
    /// <param name="result">The interception result that can be modified or returned.</param>
    /// <param name="cancellationToken">A token to observe if the operation should be canceled.</param>
    /// <returns>
    ///     A ValueTask representing the asynchronous operation, returning an <see cref="InterceptionResult{T}" /> of
    ///     integer.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the DbContext in eventData is null.</exception>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is null)
            return new ValueTask<InterceptionResult<int>>(result);

        DateTime timestamp = _timeProvider.GetUtcNow().UtcDateTime;
        string? userId = _userIdAccessor.TryGetUserId();

        List<AuditEntry> auditEntries = [];

        foreach (EntityEntry<IIdentifiableEntity> entityEntry in eventData.Context.ChangeTracker
                     .Entries<IIdentifiableEntity>())
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    if (entityEntry.Entity is IAuditableEntity)
                        SetAuditBeforeAdd(entityEntry, userId, timestamp);
                    break;
                case EntityState.Modified:
                    if (entityEntry.Entity is IAuditableEntity)
                        SetAuditBeforeUpdate(entityEntry, userId, timestamp);
                    break;
                case EntityState.Deleted:
                    switch (entityEntry.Entity)
                    {
                        case ISoftDeletableEntity:
                            entityEntry.State = EntityState.Modified;
                            entityEntry.CurrentValues[nameof(ISoftDeletable<Guid>.IsDeleted)] = true;
                            break;
                        case IAuditableEntity:
                            SetAuditBeforeDelete(entityEntry, userId, timestamp);
                            break;
                    }

                    break;
            }

            if (!_auditOptionsExtension.UseAuditEntry)
                continue;

            AuditEntry? auditEntry = entityEntry.ToAuditEntry();
            if (auditEntry is not null)
                auditEntries.Add(auditEntry);
        }

        // ReSharper disable once InvertIf
        if (_auditOptionsExtension.UseAuditEntry)
        {
            eventData.Context.Set<AuditEntry>().AddRange(auditEntries);
            auditEntries.Clear();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    ///     Sets the audit metadata for an entity being added to the database, including
    ///     the creator's identifier and the timestamp of creation.
    /// </summary>
    /// <param name="entityEntry">The entity entry representing the entity being added.</param>
    /// <param name="createdBy">The identifier of the user who created the entity. Can be null if the user is not available.</param>
    /// <param name="createdAt">The timestamp when the entity was created.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="entityEntry" /> is null.</exception>
    /// <remarks>
    ///     This method updates the "CreatedBy" and "CreatedAt" properties of an entity if it implements
    ///     <see cref="IAuditableEntity" />.
    /// </remarks>
    private static void SetAuditBeforeAdd(EntityEntry entityEntry, string? createdBy, DateTime createdAt)
    {
        entityEntry.CurrentValues[nameof(IAuditableEntity.CreatedBy)] = createdBy;
        entityEntry.CurrentValues[nameof(IAuditableEntity.CreatedAt)] = createdAt;
    }

    /// <summary>
    ///     Updates the audit information of an entity before it is saved with modifications.
    ///     Specifically, it sets the UpdatedBy and UpdatedAt properties of the entity.
    /// </summary>
    /// <param name="entityEntry">
    ///     The entity entry being updated. It contains the current state and values of the entity.
    /// </param>
    /// <param name="updatedBy">
    ///     The identifier of the user making the update, typically a user ID or name. Can be null.
    /// </param>
    /// <param name="updatedAt">
    ///     The timestamp indicating when the update occurred.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the entityEntry parameter is null.
    /// </exception>
    private static void SetAuditBeforeUpdate(EntityEntry entityEntry, string? updatedBy, DateTime updatedAt)
    {
        entityEntry.CurrentValues[nameof(IAuditableEntity.UpdatedBy)] = updatedBy;
        entityEntry.CurrentValues[nameof(IAuditableEntity.UpdatedAt)] = updatedAt;
    }

    /// <summary>
    ///     Updates the audit properties of an entity to reflect deletion metadata, including the user who deleted
    ///     and the timestamp of deletion. Primarily used for entities implementing <see cref="IAuditableEntity" />.
    /// </summary>
    /// <param name="entityEntry">The <see cref="EntityEntry" /> representing the entity to be updated for deletion.</param>
    /// <param name="deletedBy">The identifier of the user performing the deletion. Can be null if user context is unavailable.</param>
    /// <param name="deletedAt">The timestamp indicating when the deletion occurred.</param>
    private static void SetAuditBeforeDelete(EntityEntry entityEntry, string? deletedBy, DateTime deletedAt) =>
        SetAuditBeforeUpdate(entityEntry, deletedBy, deletedAt);
}