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

using Carcass.Core;
using EventStore.Client;

namespace Carcass.Data.EventStoreDb.Extensions;

/// <summary>
///     Provides extension methods for working with <see cref="EventRecord" /> instances.
/// </summary>
public static class EventRecordExtensions
{
    /// <summary>
    ///     Retrieves the event number from an <see cref="EventRecord" /> instance and adjusts it by adding one.
    /// </summary>
    /// <param name="eventRecord">The <see cref="EventRecord" /> containing the event data.</param>
    /// <returns>The adjusted event number as a long value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="eventRecord" /> is null.</exception>
    public static long GetEventNumber(this EventRecord eventRecord)
    {
        ArgumentVerifier.NotNull(eventRecord, nameof(eventRecord));

        return eventRecord.EventNumber.ToInt64() + 1; // EventStoreDB record starts with position 0.
    }
}