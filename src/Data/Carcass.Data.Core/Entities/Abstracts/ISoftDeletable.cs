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
///     Defines properties and behavior for entities that support soft deletion functionality.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
public interface ISoftDeletable<TId> : IIdentifiable<TId>
{
    /// <summary>
    ///     Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    /// <remarks>
    ///     This property implements a soft delete mechanism, allowing entities to be
    ///     marked as deleted without physically removing them from the underlying data storage.
    /// </remarks>
    /// <value>
    ///     A boolean where <c>true</c> indicates the entity is deleted and <c>false</c> indicates it is active.
    /// </value>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the property is improperly configured or used inappropriately.
    /// </exception>
    bool IsDeleted { get; set; }
}