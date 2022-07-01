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

using Carcass.ObjectStorage.Core.Models.Inputs.Buckets;
using Carcass.ObjectStorage.Core.Models.Inputs.Objects;
using Carcass.ObjectStorage.Core.Models.Outputs.Buckets;
using Carcass.ObjectStorage.Core.Models.Outputs.Objects;

namespace Carcass.ObjectStorage.Core.Providers.Abstracts;

public interface IObjectStorageProvider
{
    Task MakeBucketAsync(
        MakeBucketInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task RemoveBucketAsync(
        RemoveBucketInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task<ExistsBucketInfoOutput> ExistsBucketAsync(
        ExistsBucketInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task PutObjectAsync(
        PutObjectInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task RemoveObjectAsync(
        RemoveObjectInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task<ListObjectsInfoOutput> ListObjectsAsync(
        ListObjectsInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task<GetObjectInfoOutput> GetObjectAsync(
        GetObjectInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task<ExistsObjectInfoOutput> ExistsObjectAsync(
        ExistsObjectInfoInput input,
        CancellationToken cancellationToken = default
    );

    Task<PutObjectInfoPresignedUrlOutput> PutObjectPresignedUrlAsync(
        PutObjectInfoPresignedUrlInput input,
        CancellationToken cancellationToken = default
    );

    Task<GetObjectInfoPresignedUrlOutput> GetObjectPresignedUrlAsync(
        GetObjectInfoPresignedUrlInput input,
        CancellationToken cancellationToken = default
    );
}