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

using Carcass.Data.Core.Entities.Abstracts;

// ReSharper disable UnusedMemberInSuper.Global

namespace Carcass.Data.Core.Audit.Abstracts;

/// <summary>
///     Represents an audit entry that tracks changes made to an entity during a specific operation.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the audit entry.</typeparam>
public interface IAuditEntry<TId> : IIdentifiable<TId>
{
    /// <summary>
    ///     Gets or sets the name of the entity associated with the audit entry.
    /// </summary>
    /// <returns>
    ///     The name of the entity as a string, or null if not set.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the entity name is accessed before being initialized in certain implementations or contexts.
    /// </exception>
    public string? EntityName { get; }

    /// <summary>
    ///     Gets or sets the primary key of the audited entity.
    /// </summary>
    /// <value>
    ///     The primary key as a string, representing the identifier of the entity being audited.
    ///     This property may be null if the primary key is not set or applicable.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to retrieve or manipulate the <c>PrimaryKey</c> if the implementation
    ///     does not support this operation.
    /// </exception>
    public string? PrimaryKey { get; }

    /// <summary>
    ///     Represents the type of operation performed on an entity.
    /// </summary>
    /// <remarks>
    ///     This property indicates if the operation performed on the entity
    ///     was a creation, update, or deletion.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Raised if the operation type is not correctly handled or supported during processing.
    /// </exception>
    public OperationType OperationType { get; }

    /// <summary>
    ///     Represents the timestamp of when the operation associated with the audit entry occurred.
    /// </summary>
    /// <value>
    ///     A <see cref="DateTime" /> indicating the precise date and time the operation was performed.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to set an invalid or uninitialized timestamp.
    /// </exception>
    public DateTime Timestamp { get; }

    /// <summary>
    ///     Represents a collection of key-value pairs where the keys are property names
    ///     and the values are the original values of those properties before a change occurred.
    /// </summary>
    /// <remarks>
    ///     This collection is useful for tracking changes made to an entity during auditing
    ///     processes and can help identify what specific values were altered.
    /// </remarks>
    /// <returns>
    ///     A dictionary where the keys are string property names and the values
    ///     are the original property values before the change.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown if an attempt is made to assign a null collection to this property.
    /// </exception>
    public Dictionary<string, object?> OldValues { get; }

    /// <summary>
    ///     Represents a collection of key-value pairs that describe the new values
    ///     associated with an entity after an operation, such as creation or update.
    /// </summary>
    /// <value>
    ///     A dictionary where the key is the property name of the entity, and
    ///     the value is the new value assigned to that property.
    /// </value>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the property fails to be retrieved or assigned under
    ///     certain conditions.
    /// </exception>
    public Dictionary<string, object?> NewValues { get; }

    /// <summary>
    ///     Gets or sets additional metadata associated with the audit entry.
    /// </summary>
    /// <remarks>
    ///     The metadata represents a collection of key-value pairs that provide additional
    ///     contextual information about the audit entry. This information can be used
    ///     to store custom details that may not be captured by other properties of the audit entry.
    /// </remarks>
    /// <value>
    ///     A dictionary of key-value pairs, where the key is a string representing the metadata key,
    ///     and the value is an object representing the metadata value.
    /// </value>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown if an operation attempts to assign a null value to the Metadata property.
    /// </exception>
    public Dictionary<string, object?> Metadata { get; }
}