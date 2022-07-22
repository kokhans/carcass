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

using System.Reactive.Linq;
using Carcass.Core;
using Carcass.Logging.Core.Adapters;
using Carcass.Logging.Core.Adapters.Abstracts;
using Carcass.ObjectStorage.Core.Models.Inputs.Buckets;
using Carcass.ObjectStorage.Core.Models.Inputs.Objects;
using Carcass.ObjectStorage.Core.Models.Outputs.Buckets;
using Carcass.ObjectStorage.Core.Models.Outputs.Objects;
using Carcass.ObjectStorage.Minio.Conductors.Abstracts;
using Carcass.ObjectStorage.Minio.Providers.Abstracts;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;

namespace Carcass.ObjectStorage.Minio.Providers;

public sealed class MinioProvider : IMinioProvider
{
    private readonly LoggerAdapter<MinioProvider> _loggerAdapter;
    private readonly IMinioConductor _minioConductor;

    public MinioProvider(
        ILoggerAdapterFactory loggerAdapterFactory,
        IMinioConductor minioConductor
    )
    {
        ArgumentVerifier.NotNull(loggerAdapterFactory, nameof(loggerAdapterFactory));
        ArgumentVerifier.NotNull(minioConductor, nameof(minioConductor));

        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<MinioProvider>();
        _minioConductor = minioConductor;
    }

    public async Task MakeBucketAsync(
        MakeBucketInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        await _minioConductor.Instance.MakeBucketAsync(
            new MakeBucketArgs().WithBucket(input.BucketName),
            cancellationToken: cancellationToken
        );
    }

    public async Task RemoveBucketAsync(
        RemoveBucketInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        await _minioConductor.Instance.RemoveBucketAsync(
            new RemoveBucketArgs().WithBucket(input.BucketName),
            cancellationToken
        );
    }

    public async Task<ExistsBucketInfoOutput> ExistsBucketAsync(
        ExistsBucketInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        bool isExists = await _minioConductor.Instance.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(input.BucketName),
            cancellationToken
        );

        return new ExistsBucketInfoOutput(isExists);
    }

    public async Task PutObjectAsync(
        PutObjectInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        if (input.FormFile.Length > 0)
        {
            await using Stream stream = input.FormFile.OpenReadStream();
            await _minioConductor.Instance
                .PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(input.BucketName)
                        .WithObject(input.ObjectName)
                        .WithStreamData(stream)
                        .WithObjectSize(stream.Length)
                        .WithContentType(input.FormFile.ContentType),
                    cancellationToken: cancellationToken
                );
        }
    }


    public async Task RemoveObjectAsync(
        RemoveObjectInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        await _minioConductor.Instance
            .RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(input.BucketName)
                    .WithObject(input.ObjectName),
                cancellationToken
            );
    }

    public async Task<ListObjectsInfoOutput> ListObjectsAsync(
        ListObjectsInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        try
        {
            IList<Item> items = new List<Item>();
            IObservable<Item> observable = _minioConductor.Instance.ListObjectsAsync(
                new ListObjectsArgs()
                    .WithBucket(input.BucketName)
                    .WithPrefix(input.Prefix)
                    .WithRecursive(input.Recursive),
                cancellationToken
            );
            IDisposable subscription = observable.Subscribe(
                i => items.Add(i),
                e => throw e
            );
            await observable.GetAwaiter();
            subscription.Dispose();

            return new ListObjectsInfoOutput(items.Select(i => new ObjectInfoOutput
                    (
                        input.BucketName,
                        i.Key,
                        i.Size,
                        i.IsDir,
                        i.LastModifiedDateTime
                    )
                ).ToList()
            );
        }
        catch (InvalidOperationException)
        {
            _loggerAdapter.LogWarning("Bucket {0} is empty.", input.BucketName);

            return new ListObjectsInfoOutput();
        }
    }

    public async Task<GetObjectInfoOutput> GetObjectAsync(
        GetObjectInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        byte[] bytes = Array.Empty<byte>();
        await _minioConductor.Instance
            .GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(input.BucketName)
                    .WithObject(input.ObjectName)
                    .WithCallbackStream(s =>
                        {
                            using MemoryStream memoryStream = new();
                            s.CopyTo(memoryStream);
                            bytes = memoryStream.ToArray();
                        }
                    ),
                cancellationToken: cancellationToken
            );

        return new GetObjectInfoOutput(bytes);
    }

    public async Task<ExistsObjectInfoOutput> ExistsObjectAsync(
        ExistsObjectInfoInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        try
        {
            await _minioConductor.Instance
                .StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(input.BucketName)
                        .WithObject(input.ObjectName),
                    cancellationToken: cancellationToken
                );

            return new ExistsObjectInfoOutput(true);
        }
        catch (ObjectNotFoundException)
        {
            _loggerAdapter.LogWarning("Object {0} not found.", input.GetObjectFullPath());

            return new ExistsObjectInfoOutput(false);
        }
    }

    public async Task<GetObjectInfoPresignedUrlOutput> GetObjectPresignedUrlAsync(
        GetObjectInfoPresignedUrlInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        try
        {
            string url = await _minioConductor.Instance
                .PresignedGetObjectAsync(new PresignedGetObjectArgs()
                    .WithBucket(input.BucketName)
                    .WithObject(input.ObjectName)
                    .WithExpiry(input.ExpiresInSeconds)
                );

            return new GetObjectInfoPresignedUrlOutput(url);
        }
        catch (ObjectNotFoundException exception)
        {
            _loggerAdapter.LogError(
                "Object {0} not found.",
                input.GetObjectFullPath(),
                exception: exception
            );
            throw;
        }
    }

    public async Task<PutObjectInfoPresignedUrlOutput> PutObjectPresignedUrlAsync(
        PutObjectInfoPresignedUrlInput input,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(input, nameof(input));

        try
        {
            string url = await _minioConductor.Instance
                .PresignedPutObjectAsync(new PresignedPutObjectArgs()
                    .WithBucket(input.BucketName)
                    .WithObject(input.ObjectName)
                    .WithExpiry(input.ExpiresInSeconds)
                );

            return new PutObjectInfoPresignedUrlOutput(url);
        }
        catch (ObjectNotFoundException exception)
        {
            _loggerAdapter.LogError(
                "Object {0} not found.",
                input.GetObjectFullPath(),
                exception: exception
            );
            throw;
        }
    }
}