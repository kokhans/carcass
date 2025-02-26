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

using Carcass.Data.Core.Audit;
using Carcass.Data.Core.Audit.Abstracts;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Carcass.Data.EntityFrameworkCore.Audit;

/// <summary>
///     Represents an audit entry that stores information about changes made to an entity.
/// </summary>
public sealed class AuditEntry : IAuditEntry<Guid>, IIdentifiableEntity
{
    /// <summary>
    ///     Gets or sets the unique identifier for the audit entry.
    /// </summary>
    /// <value>
    ///     A <see cref="Guid" /> representing the unique identifier of the audit entry.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the ID is set to a null or invalid value.
    /// </exception>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the entity associated with the audit entry.
    /// </summary>
    /// <remarks>
    ///     The property represents the database table name or equivalent entity name that is being tracked for auditing
    ///     purposes.
    /// </remarks>
    /// <value>
    ///     A string representing the name of the entity. Can be null if not set.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an operation attempts to access this property in a scenario where the entity name is not available.
    /// </exception>
    public string? EntityName { get; set; }

    /// <summary>
    ///     Represents the primary key value of the audited entity as a string.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the key of the entity being audited in string format,
    ///     allowing for general-purpose handling regardless of the actual key type.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the primary key cannot be determined from the entity.
    /// </exception>
    public string? PrimaryKey { get; set; }

    /// <summary>
    ///     Represents the type of operation performed during the audit process.
    /// </summary>
    /// <remarks>
    ///     The <c>OperationType</c> property indicates the type of change performed
    ///     on the audited entity, such as creating, updating, or deleting the entity.
    /// </remarks>
    /// <value>
    ///     A value of the <c>OperationType</c> enumeration indicating the operation type.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the value assigned to <c>OperationType</c> is null and must be specified.
    /// </exception>
    public OperationType OperationType { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp indicating the date and time when the audit entry was created or occurred.
    /// </summary>
    /// <remarks>
    ///     This property is generally set to the current UTC date and time during the creation of the audit entry.
    /// </remarks>
    /// <value>
    ///     A <see cref="DateTime" /> object that represents the UTC date and time of the audit entry event.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when an attempt is made to set the value to a <see cref="DateTime" /> that falls outside the valid range.
    /// </exception>
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Gets or sets a dictionary containing the original values of the entity's properties
    ///     before a modification took place.
    /// </summary>
    /// <remarks>
    ///     The dictionary keys represent property names, while the values represent
    ///     the corresponding original property values before the modification.
    /// </remarks>
    /// <value>
    ///     A dictionary of string keys and object values containing the old state of the entity's properties.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when attempting to set a null value to this property.
    /// </exception>
    public Dictionary<string, object?> OldValues { get; set; } = [];

    /// <summary>
    ///     Contains the new values of the modified entity's properties during an audit operation.
    /// </summary>
    /// <remarks>
    ///     This property is a dictionary where the keys represent the names of the properties within the entity
    ///     and the values represent their corresponding new values after the modification.
    /// </remarks>
    /// <value>
    ///     Returns a <see cref="Dictionary{TKey, TValue}" /> where the key is a string representing the name of a property,
    ///     and the value is an object that holds the updated value of the property after an operation has occurred.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when trying to assign a null dictionary to this property.
    /// </exception>
    public Dictionary<string, object?> NewValues { get; set; } = [];

    /// <summary>
    ///     Represents additional context or information related to an audit entry.
    ///     This property is a collection of key-value pairs, where the keys are string identifiers
    ///     and the values can represent any associated metadata objects or information.
    /// </summary>
    /// <remarks>
    ///     This property is typically used to store supplemental details such as transaction IDs,
    ///     correlation IDs, or any other relevant contextual information that may enhance the audit log.
    /// </remarks>
    /// <value>
    ///     A dictionary containing metadata entries associated with the audit operation.
    ///     The keys are of type <see cref="string" />, and the values can be of any object type.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the dictionary object assigned to this property is null.
    /// </exception>
    public Dictionary<string, object?> Metadata { get; set; } = [];
}