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
///     Represents an entity that has a unique identifier and is associated with a specific tenant.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
public interface ITenantifiable<TId> : IIdentifiable<TId>
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Represents the tenant identifier associated with an entity.
    /// </summary>
    /// <remarks>
    ///     This property is used to associate a specific entity with a tenant in a multi-tenant architecture.
    /// </remarks>
    /// <value>
    ///     A string containing the tenant's unique identifier. It can be null.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when operations dependent on the tenant ID are invoked and the value is not properly configured.
    /// </exception>
    public string? TenantId { get; set; }
}