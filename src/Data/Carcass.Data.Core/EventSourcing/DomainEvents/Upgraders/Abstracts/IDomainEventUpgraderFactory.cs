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

namespace Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;

/// <summary>
///     Defines the method for retrieving a domain event upgrader for a specific domain event type.
/// </summary>
public interface IDomainEventUpgraderFactory
{
    /// <summary>
    ///     Retrieves an implementation of <see cref="IDomainEventUpgrader" /> for upgrading
    ///     a specific type of domain event.
    /// </summary>
    /// <param name="domainEventType">The type of the domain event that needs to be upgraded.</param>
    /// <returns>
    ///     An instance of <see cref="IDomainEventUpgrader" /> capable of upgrading the specified
    ///     domain event type, or <c>null</c> if no suitable upgrader is available.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="domainEventType" /> is <c>null</c>.
    /// </exception>
    IDomainEventUpgrader? GetDomainEventUpgrader(Type domainEventType);
}