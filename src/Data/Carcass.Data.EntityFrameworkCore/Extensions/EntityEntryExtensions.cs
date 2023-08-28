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

using Carcass.Core;
using Carcass.Data.Core.Audit;
using Carcass.Data.EntityFrameworkCore.Audit;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Carcass.Core.Accessors.CorrelationId.Abstracts;
using Carcass.Core.Locators;

namespace Carcass.Data.EntityFrameworkCore.Extensions;

public static class EntityEntryExtensions
{
    public static AuditEntry? ToAuditEntry(this EntityEntry entityEntry)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        AuditEntry auditEntry = new()
        {
            EntityName = entityEntry.Metadata.GetTableName(),
            Timestamp = Clock.Current.UtcNow
        };

        switch (entityEntry.State)
        {
            case EntityState.Added:
                auditEntry.OperationType = OperationType.Created;
                break;
            case EntityState.Modified:
                auditEntry.OperationType = OperationType.Updated;
                break;
            case EntityState.Deleted:
                auditEntry.OperationType = OperationType.Deleted;
                break;

            default:
                return null;
        }

        foreach (PropertyEntry propertyEntry in entityEntry.Properties)
        {
            object? currentValue = propertyEntry.CurrentValue;

            if (propertyEntry.Metadata.IsPrimaryKey())
            {
                auditEntry.PrimaryKey = currentValue?.ToString();
                continue;
            }

            string propertyName = propertyEntry.Metadata.Name;
            object? originalValue = propertyEntry.OriginalValue;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    auditEntry.NewValues[propertyName] = currentValue;
                    break;
                case EntityState.Modified:
                    if (propertyEntry.IsModified)
                    {
                        if (originalValue is not null && !originalValue.Equals(currentValue) || originalValue is null && currentValue is not null)
                        {
                            auditEntry.OldValues[propertyName] = originalValue;
                            auditEntry.NewValues[propertyName] = currentValue;
                        }
                    }

                    break;
            }
        }

        Guid? transactionId = entityEntry.Context.Database.CurrentTransaction?.TransactionId;
        if (!string.IsNullOrWhiteSpace(transactionId?.ToString()))
            auditEntry.Metadata.Add("TransactionId", transactionId);

        ICorrelationIdAccessor correlationIdAccessor =
            ServiceProviderLocator.Current.GetRequiredService<ICorrelationIdAccessor>();
        string? correlationId = correlationIdAccessor.TryGetCorrelationId();
        if (!string.IsNullOrWhiteSpace(correlationId))
            auditEntry.Metadata.Add("CorrelationId", correlationId);

        return auditEntry;
    }
}