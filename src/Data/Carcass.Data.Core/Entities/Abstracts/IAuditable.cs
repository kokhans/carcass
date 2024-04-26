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

namespace Carcass.Data.Core.Entities.Abstracts;

/// <summary>
///     Represents an auditable entity, containing metadata regarding its creation and updates.
/// </summary>
/// <typeparam name="TId">The type of the identifier for the entity.</typeparam>
public interface IAuditable<TId> : IIdentifiable<TId>
{
    /// <summary>
    ///     Gets or sets the identifier of the user or entity that created the object.
    /// </summary>
    /// <value>
    ///     The unique identifier of the creator, which can be null if not set.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Throws if there is an issue setting or retrieving the value.
    /// </exception>
    string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the entity was created.
    /// </summary>
    /// <value>
    ///     Represents the creation timestamp of the entity.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to set an invalid value.
    /// </exception>
    DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    /// <value>
    ///     A string representing the identifier of the last user who modified the entity.
    ///     This value can be null if the entity has not been updated or the information is unavailable.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an attempt is made to retrieve or set the value in an invalid entity state.
    /// </exception>
    string? UpdatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the entity was last updated.
    /// </summary>
    /// <remarks>
    ///     This property captures the timestamp of the most recent update operation on the entity.
    ///     If the entity has not been updated since creation, the value may be null.
    /// </remarks>
    /// <value>
    ///     A nullable <see cref="DateTime" /> representing the last modification timestamp.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the property is accessed or set in an invalid state.
    /// </exception>
    DateTime? UpdatedAt { get; set; }
}