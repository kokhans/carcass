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

using Carcass.Media.Core.Models;
using Carcass.Media.Core.Models.Output;

namespace Carcass.Media.Core.Providers.Abstracts;

/// <summary>
///     Provides an abstraction for media uploading functionality to different storage services.
/// </summary>
public interface IMediaProvider
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Uploads a media file to the specified folder and returns the details of the uploaded file.
    /// </summary>
    /// <param name="folderName">The name of the folder where the media will be uploaded.</param>
    /// <param name="fileName">The name of the file to be uploaded.</param>
    /// <param name="stream">The data stream of the file to be uploaded.</param>
    /// <param name="mediaType">The type of the media being uploaded (e.g., Video, Image, File).</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An object containing the details of the uploaded media.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the fileName or stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if folderName or fileName is invalid.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the media upload fails due to invalid configuration or other
    ///     issues.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the cancellationToken.</exception>
    Task<UploadMediaOutput> UploadMediaAsync(
        string folderName,
        string fileName,
        Stream stream,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    );
}