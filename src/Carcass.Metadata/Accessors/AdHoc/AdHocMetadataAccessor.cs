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

using System.Collections.ObjectModel;
using Carcass.Core;
using Carcass.Metadata.Accessors.AdHoc.Abstracts;
using Carcass.Metadata.Options;
using Carcass.Metadata.Stores.Abstracts;
using Microsoft.Extensions.Options;

namespace Carcass.Metadata.Accessors.AdHoc;

/// <summary>
///     Provides functionality for managing and accessing ad-hoc metadata dynamically.
///     Enables the creation of additional metadata that can be merged with existing metadata from a store.
/// </summary>
public sealed class AdHocMetadataAccessor : IAdHocMetadataAccessor
{
    /// <summary>
    ///     A private dictionary used to store key-value pairs of ad-hoc metadata.
    ///     This metadata is managed by the <see cref="AdHocMetadataAccessor" /> class and
    ///     provides a mechanism for storing and resolving additional metadata that might
    ///     not be persisted in the primary metadata store.
    /// </summary>
    /// <remarks>
    ///     The behavior of metadata resolution is determined by the
    ///     <see cref="AdHocMetadataAccessorOptions.ResolutionStrategy" /> property.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when duplicate keys exist between persisted metadata and
    ///     ad-hoc metadata, and the resolution strategy is set to
    ///     <see cref="AdHocMetadataResolutionStrategy.ThrowsException" />.
    /// </exception>
    private readonly Dictionary<string, object?> _adHocMetadata;

    /// <summary>
    ///     Represents an instance of <see cref="IMetadataStore" /> used for managing metadata storage and retrieval.
    /// </summary>
    /// <remarks>
    ///     This field is used for interacting with the underlying metadata store in operations such as retrieving, updating,
    ///     or deleting metadata. It is a primary dependency for enabling metadata management functionality in the class.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when an attempt is made to initialize this dependency with a null reference.
    /// </exception>
    private readonly IMetadataStore _metadataStore;

    /// <summary>
    ///     Provides access to the options for configuring behaviors associated with ad-hoc metadata resolution.
    /// </summary>
    /// <remarks>
    ///     This member holds an instance of <see cref="IOptions{TOptions}" /> for <see cref="AdHocMetadataAccessorOptions" />,
    ///     which defines the strategy used to resolve conflicts between ad-hoc and persisted metadata.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown during metadata resolution if a conflict occurs and the resolution strategy is set to
    ///     <see cref="AdHocMetadataResolutionStrategy.ThrowsException" />.
    /// </exception>
    private readonly IOptions<AdHocMetadataAccessorOptions> _optionsAccessor;

    /// <summary>
    ///     Provides a concrete implementation for managing ad-hoc metadata functionality, allowing
    ///     for dynamic addition and retrieval of metadata through an <see cref="IMetadataStore" />.
    /// </summary>
    public AdHocMetadataAccessor(
        IOptions<AdHocMetadataAccessorOptions> optionsAccessor,
        IMetadataStore metadataStore
    )
    {
        ArgumentVerifier.NotNull(optionsAccessor, nameof(optionsAccessor));
        ArgumentVerifier.NotNull(metadataStore, nameof(metadataStore));

        _optionsAccessor = optionsAccessor;
        _metadataStore = metadataStore;
        _adHocMetadata = new Dictionary<string, object?>();
    }

    /// <summary>
    ///     Adds or updates ad-hoc metadata with the specified key and value.
    /// </summary>
    /// <param name="key">The key associated with the metadata. Must not be null or empty.</param>
    /// <param name="value">The value of the metadata. Can be null.</param>
    /// <returns>The current instance of <see cref="IAdHocMetadataAccessor" /> to allow method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="key" /> is null or empty.</exception>
    public IAdHocMetadataAccessor WithAdHocMetadata(string key, object? value)
    {
        ArgumentVerifier.NotNull(key, nameof(key));

        _adHocMetadata.Add(key, value);

        return this;
    }

    /// <summary>
    ///     Asynchronously retrieves a combined set of metadata, including both persisted and ad-hoc metadata.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. If cancellation is requested, an exception will be thrown.
    /// </param>
    /// <returns>
    ///     A read-only dictionary containing the combined set of metadata.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when a conflict occurs between persisted and ad-hoc metadata keys
    ///     and the resolution strategy is set to throw an exception.
    /// </exception>
    public async Task<ReadOnlyDictionary<string, object?>> GetMetadataAsync(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ReadOnlyDictionary<string, object?> metadata = await _metadataStore.GetMetadataAsync(cancellationToken);

        if (_adHocMetadata.Count == 0)
            return metadata;

        foreach (KeyValuePair<string, object?> item in metadata)
        {
            if (_adHocMetadata.ContainsKey(item.Key))
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (_optionsAccessor.Value.ResolutionStrategy)
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