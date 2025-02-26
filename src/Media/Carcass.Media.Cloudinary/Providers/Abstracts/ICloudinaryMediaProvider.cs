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
using Carcass.Media.Core.Providers.Abstracts;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Carcass.Media.Cloudinary.Providers.Abstracts;

/// <summary>
///     Provides an interface for interacting with Cloudinary media services,
///     supporting operations for retrieving, downloading, deleting, moving, and renaming media files.
/// </summary>
public interface ICloudinaryMediaProvider : IMediaProvider
{
    /// <summary>
    ///     Asynchronously retrieves media information based on the specified public identifier and media type.
    /// </summary>
    /// <param name="publicId">The unique identifier of the media to retrieve.</param>
    /// <param name="mediaType">The type of media to retrieve, such as Image, Video, or File.</param>
    /// <param name="cancellationToken">A cancellation token used to cancel the operation before it completes, if required.</param>
    /// <returns>A <see cref="GetMediaOutput" /> object containing details about the requested media.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided publicId is null, empty, or invalid.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when media retrieval fails or the specified media type is
    ///     unsupported.
    /// </exception>
    /// <exception cref="TaskCanceledException">Thrown if the operation is canceled via the provided CancellationToken.</exception>
    Task<GetMediaOutput> GetMediaAsync(
        string publicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Downloads media from Cloudinary based on the specified public ID and media type.
    /// </summary>
    /// <param name="publicId">
    ///     The unique identifier of the media to be downloaded.
    /// </param>
    /// <param name="mediaType">
    ///     The type of media to be downloaded (e.g., Image, Video, File).
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests. Optional parameter; if not provided, the default value is used.
    /// </param>
    /// <returns>
    ///     A byte array containing the media content.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the provided publicId is null or empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the specified media type is unsupported or undefined.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown if the download operation fails due to a Cloudinary-related error.
    /// </exception>
    Task<byte[]> DownloadMediaAsync(
        string publicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Deletes media resources from the cloud storage using the provided public IDs and media type.
    /// </summary>
    /// <param name="publicIds">An array of public IDs identifying the media resources to be deleted.</param>
    /// <param name="mediaType">The type of the media (e.g., Video, Image, File) to be deleted.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the delete operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="publicIds" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the delete operation cannot be completed due to invalid input or
    ///     state.
    /// </exception>
    Task DeleteAsync(
        string[] publicIds,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Moves a list of media items from their current public IDs to a specified target public ID.
    /// </summary>
    /// <param name="fromPublicIds">The list of public IDs representing the source media items to be moved. Can be null.</param>
    /// <param name="toPublicId">The target public ID where the media items should be moved.</param>
    /// <param name="mediaType">The type of media being moved, such as video, image, or file.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing a list of <see cref="MoveMediaOutput" /> objects
    ///     with the details of the operation's outcome.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="toPublicId" /> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="mediaType" /> is undefined.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task<IList<MoveMediaOutput>> MoveMediaAsync(
        IList<string>? fromPublicIds,
        string toPublicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Renames a media resource by updating its public ID in the Cloudinary storage.
    /// </summary>
    /// <param name="fromPublicId">The current public ID of the media resource to be renamed.</param>
    /// <param name="toPublicId">The new public ID to assign to the media resource.</param>
    /// <param name="mediaType">The type of the media resource, such as Image, Video, or File.</param>
    /// <param name="cancellationToken">An optional token to observe cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="fromPublicId" /> or <paramref name="toPublicId" /> is null or empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the renaming operation fails or the specified media resource does not exist.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    Task RenameMediaAsync(
        string fromPublicId,
        string toPublicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    );
}