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
using Carcass.Core.Extensions;
using Carcass.Media.Abstracts;
using Carcass.Media.Abstracts.Models.Output;
using Carcass.Media.Cloudinary.Providers.Abstracts;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Carcass.Media.Cloudinary.Providers;

public sealed class CloudinaryMediaProvider : ICloudinaryMediaProvider
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    private readonly HttpClient _httpClient;

    public CloudinaryMediaProvider(CloudinaryDotNet.Cloudinary cloudinary)
    {
        ArgumentVerifier.NotNull(cloudinary, nameof(cloudinary));

        _cloudinary = cloudinary;
        _httpClient = new HttpClient();
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

        ImageUploadResult result = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            PublicId = $"{folderName}/{fileName}",
            File = new FileDescription(fileName, stream)
        }, cancellationToken);

        if (!result.StatusCode.IsSuccessHttpStatusCode())
            throw new Exception($"Upload {mediaType.ToString().ToLowerInvariant()} {fileName} failed.");

        UploadMediaOutput output = new()
        {
            FolderName = folderName,
            FileName = fileName,
            Extension = result.Format,
            Uri = result.Url,
            Size = result.Bytes,
            CreatedAt = result.CreatedAt,
            MediaType = GetMediaType(result.ResourceType)
        };
        output.Metadata.Add("public-id", result.PublicId);

        return output;
    }

    public async Task<GetMediaOutput> GetMediaAsync(
        string publicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(publicId, nameof(publicId));

        GetResourceResult result = await _cloudinary.GetResourceAsync(new GetResourceParams(publicId)
        {
            ResourceType = GetResourceType(mediaType)
        }, cancellationToken);

        if (!result.StatusCode.IsSuccessHttpStatusCode())
            throw new Exception($"Get {publicId} failed.");

        string[] splitPublicId = publicId.Split("/");
        string folderName = string.Join("/", splitPublicId.SkipLast(1));
        string fileName = splitPublicId.Last();

        GetMediaOutput output = new()
        {
            FolderName = folderName,
            FileName = fileName,
            Extension = result.Format,
            Uri = new Uri(result.Url),
            Size = result.Bytes,
            CreatedAt = DateTime.Parse(result.CreatedAt),
            MediaType = GetMediaType(result.ResourceType)
        };
        output.Metadata.Add("public-id", result.PublicId);

        return output;
    }

    public async Task<byte[]> DownloadMediaAsync(
        string publicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(publicId, nameof(publicId));

        GetMediaOutput output = await GetMediaAsync(publicId, mediaType, cancellationToken);

        return await _httpClient.GetByteArrayAsync(output.Uri, cancellationToken);
    }

    public async Task DeleteAsync(
        string[] publicIds,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(publicIds, nameof(publicIds));

        await _cloudinary.DeleteResourcesAsync(new DelResParams
        {
            PublicIds = publicIds.ToList(),
            ResourceType = GetResourceType(mediaType)
        }, cancellationToken);
    }

    public async Task<IList<MoveMediaOutput>> MoveMediaAsync(
        IList<string>? fromPublicIds,
        string toPublicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        IList<MoveMediaOutput> attachments = new List<MoveMediaOutput>();

        if (fromPublicIds == null || !fromPublicIds.Any())
            return attachments;

        foreach (string fromPublicId in fromPublicIds)
        {
            string[] splitFromPublicId = fromPublicId.Split('/');
            string toFullPublicId = $"{toPublicId}{splitFromPublicId.Last()}";

            await RenameMediaAsync(fromPublicId, toFullPublicId, mediaType, cancellationToken);

            GetMediaOutput output = await GetMediaAsync(toFullPublicId, mediaType, cancellationToken);
            string contentType = MimeTypes.MimeTypeMap.GetMimeType(output.Extension);

            MoveMediaOutput moveMediaOutput = new()
            {
                FileName = output.FileName,
                MediaType = output.MediaType,
                Size = output.Size,
                CreatedAt = output.CreatedAt,
                ContentType = contentType,
                Uri = output.Uri,
                Extension = output.Extension,
                FolderName = output.FolderName
            };
            moveMediaOutput.Metadata.Add("public-id", output.Metadata["public-id"]);

            attachments.Add(moveMediaOutput);
        }

        return attachments;
    }

    public async Task RenameMediaAsync(
        string fromPublicId,
        string toPublicId,
        MediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(fromPublicId, nameof(fromPublicId));
        ArgumentVerifier.NotNull(toPublicId, nameof(toPublicId));

        RenameResult result = await _cloudinary.RenameAsync(new RenameParams(fromPublicId, toPublicId)
        {
            ResourceType = GetResourceType(mediaType)
        });

        if (!result.StatusCode.IsSuccessHttpStatusCode())
            throw new Exception($"Rename resource from {fromPublicId} to {toPublicId} failed.");
    }

    private static MediaType GetMediaType(string type)
    {
        ArgumentVerifier.NotNull(type, nameof(type));

        return type.ToLowerInvariant() switch
        {
            "video" => MediaType.Video,
            "image" => MediaType.Image,
            "raw" => MediaType.File,
            _ => MediaType.Undefined
        };
    }

    private static MediaType GetMediaType(ResourceType type) => GetMediaType(type.ToString());

    private static ResourceType GetResourceType(MediaType type) =>
        type switch
        {
            MediaType.Video => ResourceType.Video,
            MediaType.Image => ResourceType.Image,
            MediaType.File => ResourceType.Raw,
            _ => ResourceType.Auto
        };
}