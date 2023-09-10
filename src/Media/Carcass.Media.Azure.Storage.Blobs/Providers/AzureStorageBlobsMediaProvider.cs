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

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Carcass.Core;
using Carcass.Core.Helpers;
using Carcass.Media.Abstracts;
using Carcass.Media.Abstracts.Models.Output;
using Carcass.Media.Azure.Storage.Blobs.Providers.Abstracts;

namespace Carcass.Media.Azure.Storage.Blobs.Providers;

public sealed class AzureStorageBlobsMediaProvider : IAzureStorageBlobsMediaProvider
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageBlobsMediaProvider(BlobServiceClient blobServiceClient)
    {
        ArgumentVerifier.NotNull(blobServiceClient, nameof(blobServiceClient));

        _blobServiceClient = blobServiceClient;
    }

    public async Task<UploadMediaOutput> UploadMediaAsync(
        string folderName,
        string fileName,
        Stream stream,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(folderName, nameof(folderName));
        ArgumentVerifier.NotNull(fileName, nameof(fileName));
        ArgumentVerifier.NotNull(stream, nameof(stream));

        string? extension = fileName.GetFileExtension();
        if (string.IsNullOrWhiteSpace(extension))
            throw new Exception($"Media {fileName} has no extension.");

        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(folderName);
        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        Response<BlobContentInfo> blobContentInfoResponse = await blobClient.UploadAsync(stream, true, cancellationToken);
        if (!blobContentInfoResponse.HasValue)
            throw new Exception($"Upload media {fileName} failed.");

        Response<BlobProperties>? blobPropertiesResponse = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        if (!blobPropertiesResponse.HasValue)
            throw new Exception($"Get media {fileName} properties failed.");

        return new UploadMediaOutput
        {
            FolderName = folderName,
            FileName = fileName,
            Extension = extension,
            Uri = blobClient.Uri,
            Size = blobPropertiesResponse.Value.ContentLength,
            CreatedAt = blobPropertiesResponse.Value.CreatedOn.UtcDateTime,
            MediaType = mediaType
        };
    }

    public async Task DeleteMediaAsync(
        string folderName,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(folderName, nameof(folderName));
        ArgumentVerifier.NotNull(fileName, nameof(fileName));

        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(folderName);
        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        Response response = await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        if (response.IsError)
            throw new Exception($"Delete media {fileName} failed.");
    }
}