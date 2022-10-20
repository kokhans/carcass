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

using System.Collections.ObjectModel;
using Carcass.Core;
using Carcass.Metadata.Accessors.AdHoc.Abstracts;
using Carcass.Metadata.Accessors.AdHoc.Options;
using Carcass.Metadata.Stores.Abstracts;
using Microsoft.Extensions.Options;

namespace Carcass.Metadata.Accessors.AdHoc;

public sealed class AdHocMetadataAccessor : IAdHocMetadataAccessor
{
    private readonly AdHocMetadataAccessorOptions _options;
    private readonly IMetadataStore _metadataStore;
    private readonly Dictionary<string, object?> _adHocMetadata;

    public AdHocMetadataAccessor(IOptions<AdHocMetadataAccessorOptions> options, IMetadataStore metadataStore)
    {
        ArgumentVerifier.NotNull(options, nameof(options));
        ArgumentVerifier.NotNull(metadataStore, nameof(metadataStore));

        _options = options.Value;
        _metadataStore = metadataStore;
        _adHocMetadata = new Dictionary<string, object?>();
    }

    public IAdHocMetadataAccessor WithAdHocMetadata(string key, object? value)
    {
        ArgumentVerifier.NotNull(key, nameof(key));

        _adHocMetadata.Add(key, value);

        return this;
    }

    public async Task<ReadOnlyDictionary<string, object?>> GetMetadataAsync(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ReadOnlyDictionary<string, object?> metadata = await _metadataStore.GetMetadataAsync(cancellationToken);

        if (!_adHocMetadata.Any())
            return metadata;

        foreach (KeyValuePair<string, object?> item in metadata)
        {
            if (_adHocMetadata.ContainsKey(item.Key))
                switch (_options.ResolutionStrategy)
                {
                    case AdHocMetadataResolutionStrategy.ReplaceWithAdHoc:
                        continue;
                    case AdHocMetadataResolutionStrategy.LeavePersisted:
                        _adHocMetadata[item.Key] = item.Value;
                        continue;
                    case AdHocMetadataResolutionStrategy.ThrowsException:
                        throw new InvalidOperationException($"Ad hoc metadata contains duplicate key {item.Key}.");
                }

            _adHocMetadata.Add(item.Key, item.Value);
        }

        return new ReadOnlyDictionary<string, object?>(_adHocMetadata);
    }
}