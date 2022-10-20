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

using Carcass.Core;
using Carcass.Core.Conductors.Abstracts;
using Carcass.ObjectStorage.Minio.Conductors.Abstracts;
using Carcass.ObjectStorage.Minio.Disposers;
using Carcass.ObjectStorage.Minio.Options;
using Microsoft.Extensions.Options;
using Minio;

namespace Carcass.ObjectStorage.Minio.Conductors;

public sealed class MinioConductor : InstanceConductor<MinioOptions, MinioClient, MinioDisposer>, IMinioConductor
{
    public MinioConductor(
        IOptionsMonitor<MinioOptions> optionsMonitorAccessor,
        Func<MinioOptions, MinioClient>? factory = default
    ) : base(optionsMonitorAccessor, factory)
    {
    }

    public MinioConductor(
        IOptions<MinioOptions> optionsAccessor,
        Func<MinioOptions, MinioClient>? factory = default
    ) : base(optionsAccessor, factory)
    {
    }

    protected override MinioClient? CreateInstance(MinioOptions options)
    {
        ArgumentVerifier.NotNull(options, nameof(options));

        return new MinioClient()
            .WithEndpoint(options.Endpoint)
            .WithCredentials(options.AccessKey, options.SecretKey)
            .Build();
    }
}