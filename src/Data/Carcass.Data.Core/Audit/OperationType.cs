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

namespace Carcass.Data.Core.Audit;

/// <summary>
///     Represents an operation where an entity was deleted in the system.
/// </summary>
/// <remarks>
///     This enumeration member is used to signify that an entity has been marked for deletion
///     or has been removed from the data source.
/// </remarks>
/// <exception cref="InvalidOperationException">
///     Thrown when attempting operations on a deleted entity that are not allowed.
/// </exception>
public enum OperationType
{
    /// <summary>
    ///     Represents a creation operation in the lifecycle of an entity.
    /// </summary>
    /// <remarks>
    ///     This enumeration member indicates that a new entity has been created within
    ///     the system or added to the data source.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an invalid state transition occurs from a Created operation.
    /// </exception>
    Created = 1,

    /// <summary>
    ///     Represents an operation where an existing entity has been modified or updated.
    /// </summary>
    /// <remarks>
    ///     This enumeration value is typically used to log or track changes made to an entity
    ///     within the context of an auditing system.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the update operation cannot be recorded due to an invalid entity state.
    /// </exception>
    Updated,

    /// <summary>
    ///     Represents an operation where an entity was deleted in the system.
    /// </summary>
    /// <remarks>
    ///     This enumeration member indicates that an entity has been removed or is no longer active
    ///     within the data context, typically corresponding to a "soft" or "hard" deletion.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to perform invalid actions on a deleted entity.
    /// </exception>
    Deleted
}