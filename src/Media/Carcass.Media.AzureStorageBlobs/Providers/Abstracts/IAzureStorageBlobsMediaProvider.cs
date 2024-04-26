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

using Carcass.Media.Core.Providers.Abstracts;

namespace Carcass.Media.AzureStorageBlobs.Providers.Abstracts;

/// <summary>
///     Defines the contract for managing media storage operations in Azure Storage Blobs.
///     Provides functionality to upload and delete media using Azure Blob Storage.
/// </summary>
public interface IAzureStorageBlobsMediaProvider : IMediaProvider
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Deletes a media file from the specified folder in Azure Blob Storage.
    /// </summary>
    /// <param name="folderName">The name of the folder (container) containing the media file.</param>
    /// <param name="fileName">The name of the media file to delete.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="folderName" /> or <paramref name="fileName" /> is
    ///     null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="Exception">Thrown if the deletion operation fails in the storage service.</exception>
    Task DeleteMediaAsync(
        string folderName,
        string fileName,
        CancellationToken cancellationToken = default
    );
}