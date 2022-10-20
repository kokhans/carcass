// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Carcass.Core;
using Carcass.Metadata.Stores.Abstracts;

namespace Carcass.Metadata.Stores;

public sealed class InMemoryMetadataStore : IMetadataStore
{
    private readonly ConcurrentDictionary<string, object?> _metadata;

    public InMemoryMetadataStore()
    {
        _metadata = new ConcurrentDictionary<string, object?>();
    }

    public Task AddOrUpdateMetadataAsync(string key, object? value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        _metadata.AddOrUpdate(key, value, (_, _) => value);

        return Task.CompletedTask;
    }

    public Task DeleteMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        _metadata.Remove(key, out _);

        return Task.CompletedTask;
    }

    public Task<ReadOnlyDictionary<string, object?>> GetMetadataAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new ReadOnlyDictionary<string, object?>(_metadata));
    }
}