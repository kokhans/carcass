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

namespace Carcass.Data.Core.EventSourcing.Aggregates.Attributes;

/// <summary>
///     Specifies the version of an aggregate in an event-sourced system.
///     This attribute is typically applied to aggregate root classes to denote
///     the schema version of the aggregate.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class AggregateVersionAttribute(long version) : Attribute
{
    /// <summary>
    ///     Gets the version associated with an aggregate.
    /// </summary>
    /// <value>
    ///     A <see cref="long" /> representing the version of the aggregate.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the version cannot be initialized or is unavailable.
    /// </exception>
    public long Version { get; } = version;
}