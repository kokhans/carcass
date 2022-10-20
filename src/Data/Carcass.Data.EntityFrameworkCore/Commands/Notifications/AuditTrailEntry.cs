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

using Carcass.Core;
using Carcass.Data.Core.Commands.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Carcass.Data.EntityFrameworkCore.Commands.Notifications;

public sealed record AuditTrailEntry
{
    private readonly EntityEntry _entityEntry;
    private Guid? _primaryKey;
    private readonly Dictionary<string, object?> _oldValues;
    private readonly Dictionary<string, object?> _newValues;

    public AuditTrailEntry(EntityEntry entityEntry)
    {
        ArgumentVerifier.NotNull(entityEntry, nameof(entityEntry));

        _entityEntry = entityEntry;
        _oldValues = new Dictionary<string, object?>();
        _newValues = new Dictionary<string, object?>();
    }

    private string? TableName => _entityEntry.Metadata.GetTableName();
    private Guid? TransactionId => _entityEntry.Context.Database.CurrentTransaction?.TransactionId;

    public AuditTrailTransactionalNotification ToAuditTrailTransactionalDomainEvent()
    {
        foreach (PropertyEntry propertyEntry in _entityEntry.Properties)
        {
            string propertyName = propertyEntry.Metadata.Name;
            if (propertyEntry.Metadata.IsPrimaryKey())
            {
                _primaryKey = (Guid?) propertyEntry.CurrentValue;
                continue;
            }

            switch (_entityEntry.State)
            {
                case EntityState.Added:
                    _newValues[propertyName] = propertyEntry.CurrentValue;
                    break;
                case EntityState.Modified:
                    if (propertyEntry.IsModified)
                    {
                        if (
                            propertyEntry.OriginalValue is not null
                            && !propertyEntry.OriginalValue.Equals(propertyEntry.CurrentValue)
                            || propertyEntry.OriginalValue is null && propertyEntry.CurrentValue is not null
                        )
                        {
                            _oldValues[propertyName] = propertyEntry.OriginalValue;
                            _newValues[propertyName] = propertyEntry.CurrentValue;
                        }
                    }

                    break;
            }
        }

        return new AuditTrailTransactionalNotification(
            _oldValues,
            _newValues,
            TableName,
            _primaryKey?.ToString(),
            TransactionId?.ToString()
        );
    }
}