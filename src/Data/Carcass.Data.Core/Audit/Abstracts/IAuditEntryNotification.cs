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

using MediatR;

// ReSharper disable UnusedMember.Global

namespace Carcass.Data.Core.Audit.Abstracts;

/// <summary>
///     Represents a notification that carries information about an audit entry related to a specific entity.
///     This interface is intended to be implemented by classes that provide details about entity changes
///     and operations, including type, timestamp, and corresponding value changes.
/// </summary>
public interface IAuditEntryNotification : INotification
{
    /// <summary>
    ///     Gets the name of the entity associated with the audit entry.
    /// </summary>
    /// <value>
    ///     The name of the entity being audited, or <c>null</c> if the entity name is not specified.
    /// </value>
    /// <returns>
    ///     A <see cref="string" /> representing the name of the entity.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the entity name is accessed but is unavailable or improperly initialized.
    /// </exception>
    public string? EntityName { get; }

    /// <summary>
    ///     Gets the primary key of the entity related to the audit entry.
    /// </summary>
    /// <returns>
    ///     The primary key of the entity as a string, or <c>null</c> if the primary key is not available.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the primary key cannot be retrieved due to an invalid state.
    /// </exception>
    public string? PrimaryKey { get; }

    /// <summary>
    ///     Gets the type of operation performed on an entity.
    /// </summary>
    /// <remarks>
    ///     This value represents the specific action (e.g., created, updated, or deleted)
    ///     that was taken on the entity and can be used to track and audit changes
    ///     within the system.
    /// </remarks>
    /// <returns>
    ///     Returns an <see cref="OperationType" /> enumeration indicating the type of the operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the operation type is accessed on an entity in an invalid state.
    /// </exception>
    public OperationType OperationType { get; }

    /// <summary>
    ///     Gets the timestamp indicating when the audit entry event occurred.
    /// </summary>
    /// <returns>
    ///     A <see cref="DateTime" /> representing the exact date and time of the event.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the timestamp retrieval encounters an invalid state.
    /// </exception>
    public DateTime Timestamp { get; }

    /// <summary>
    ///     Gets a dictionary of property names and their corresponding values
    ///     representing the state of the entity before the operation was performed.
    /// </summary>
    /// <returns>
    ///     A dictionary where the keys are property names and the values are the
    ///     entity's previous values prior to the operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the operation attempts to access the old values when they are
    ///     unavailable or not applicable for the audit context.
    /// </exception>
    public Dictionary<string, object?> OldValues { get; }

    /// <summary>
    ///     Represents the collection of new values for an entity's properties after an operation is performed.
    /// </summary>
    /// <remarks>
    ///     This property contains the updated property values of an entity following a create or update operation.
    ///     It provides a dictionary where the keys are property names and the values are the corresponding new values of the
    ///     entity.
    /// </remarks>
    /// <returns>
    ///     A <see cref="Dictionary{TKey, TValue}" /> containing the property names as keys and the new property values as
    ///     values.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown if an attempt is made to access a key that does not exist in the dictionary.
    /// </exception>
    public Dictionary<string, object?> NewValues { get; }

    /// <summary>
    ///     Represents additional information related to an audit entry.
    /// </summary>
    /// <remarks>
    ///     This property holds a collection of key-value pairs that provide extra
    ///     contextual metadata associated with an audit event.
    /// </remarks>
    public Dictionary<string, object?> Metadata { get; }
}