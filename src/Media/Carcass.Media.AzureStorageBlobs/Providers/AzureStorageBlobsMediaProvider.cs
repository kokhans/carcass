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

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Carcass.Core;
using Carcass.Core.Helpers;
using Carcass.Media.AzureStorageBlobs.Providers.Abstracts;
using Carcass.Media.Core.Models;
using Carcass.Media.Core.Models.Output;

namespace Carcass.Media.AzureStorageBlobs.Providers;

/// <summary>
///     Provides functionality to manage and interact with media stored in Azure Blob Storage.
/// </summary>
public sealed class AzureStorageBlobsMediaProvider : IAzureStorageBlobsMediaProvider
{
    /// <summary>
    ///     The BlobServiceClient instance used to interact with Azure Blob Storage services, such as managing containers and
    ///     blobs.
    /// </summary>
    /// <remarks>
    ///     This client is a core dependency for operations such as uploading, retrieving, and deleting blobs within a blob
    ///     storage account.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown during initialization if the provided <see cref="BlobServiceClient" /> is null.
    /// </exception>
    private readonly BlobServiceClient _blobServiceClient;

    /// <summary>
    ///     Provides functionality for handling media operations with Azure Storage Blobs.
    ///     This class is used to upload and delete media files stored in an Azure Blob Storage account.
    /// </summary>
    public AzureStorageBlobsMediaProvider(BlobServiceClient blobServiceClient)
    {
        ArgumentVerifier.NotNull(blobServiceClient, nameof(blobServiceClient));

        _blobServiceClient = blobServiceClient;
    }

    /// <summary>
    ///     Uploads a media file to Azure Blob Storage within the specified folder.
    /// </summary>
    /// <param name="folderName">The name of the folder in Azure Blob Storage where the media file will be uploaded.</param>
    /// <param name="fileName">The name of the file to be uploaded, including its extension.</param>
    /// <param name="stream">The stream containing the media file data.</param>
    /// <param name="mediaType">The type of the media file being uploaded.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An <see cref="UploadMediaOutput" /> object containing details of the uploaded file, such as its URI, size, and
    ///     creation time.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="folderName" />, <paramref name="fileName" />, or
    ///     <paramref name="stream" /> is null.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown if the file extension is missing, the upload operation fails, or the file properties cannot be retrieved.
    /// </exception>
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

        Response<BlobContentInfo> blobContentInfoResponse =
            await blobClient.UploadAsync(stream, true, cancellationToken);
        if (!blobContentInfoResponse.HasValue)
            throw new Exception($"Upload media {fileName} failed.");

        Response<BlobProperties>? blobPropertiesResponse =
            await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
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

    /// <summary>
    ///     Deletes a media file from the specified Azure Storage Blob container and handles snapshot deletion if applicable.
    /// </summary>
    /// <param name="folderName">The name of the blob container where the media file is stored.</param>
    /// <param name="fileName">The name of the file to be deleted from the container.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="folderName" /> or <paramref name="fileName" /> is null.
    /// </exception>
    /// <exception cref="RequestFailedException">
    ///     Thrown when the deletion operation encounters an error in the Azure Blob Storage service.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
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

        Response response = await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);
        if (response.IsError)
            throw new Exception($"Delete media {fileName} failed.");
    }
}