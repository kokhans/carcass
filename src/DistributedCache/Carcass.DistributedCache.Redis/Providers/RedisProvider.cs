// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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
using Carcass.DistributedCache.Redis.Providers.Abstracts;
using Carcass.Json.Core.Providers.Abstracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Carcass.DistributedCache.Redis.Providers;

public sealed class RedisProvider : IRedisProvider
{
    private readonly RedisCache _redisCache;
    private readonly IJsonProvider _jsonProvider;

    public RedisProvider(
        RedisCache redisCache,
        IJsonProvider jsonProvider
    )
    {
        ArgumentVerifier.NotNull(redisCache, nameof(redisCache));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));

        _redisCache = redisCache;
        _jsonProvider = jsonProvider;
    }

    public async Task<T?> TryGetAsync<T>(
        string key,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        string? json = await _redisCache.GetStringAsync(key, cancellationToken);

        return string.IsNullOrWhiteSpace(json)
            ? default
            : _jsonProvider.TryDeserialize<T>(json);
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        T? json = await TryGetAsync<T>(key, cancellationToken);

        return json ?? throw new InvalidOperationException("JSON is null.");
    }

    public async Task SetAsync<T>(
        string key,
        T data,
        DistributedCacheEntryOptions? distributedCacheEntryOptions = default,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        if (distributedCacheEntryOptions is null)
            await _redisCache.SetStringAsync(
            key,
            _jsonProvider.Serialize(data),
            cancellationToken
        );
        else
            await _redisCache.SetStringAsync(
                key,
                _jsonProvider.Serialize(data),
                distributedCacheEntryOptions,
                cancellationToken
            );
    }
}