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

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Carcass.Media.Core.Models.Output.Abstracts;

/// <summary>
///     Represents the base class for media-related output objects, providing common properties
///     such as file details, URI, size, created date, media type, and metadata.
/// </summary>
public abstract class MediaOutput
{
    /// <summary>
    ///     Represents the name of the folder associated with the media.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when an attempt is made to set the property to a null value without proper handling.
    /// </exception>
    /// <remarks>
    ///     This property is typically used to organize media files into specific storage locations or categories.
    /// </remarks>
    public required string FolderName { get; set; }

    /// <summary>
    ///     Represents the name of a media file associated with the output.
    /// </summary>
    /// <remarks>
    ///     The property holds the file name, including its extension, used to identify the media in contexts such as uploads,
    ///     retrievals, and processing.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the file name is set to <c>null</c>.
    /// </exception>
    public required string FileName { get; set; }

    /// <summary>
    ///     Gets or sets the file extension of the media.
    /// </summary>
    /// <remarks>
    ///     The file extension represents the format or type of the media file,
    ///     such as "jpg", "png", "mp4", etc.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when attempting to set a null or empty value.
    /// </exception>
    public required string Extension { get; set; }

    /// <summary>
    ///     Gets or sets the URI associated with the media output.
    /// </summary>
    /// <value>
    ///     A <see cref="System.Uri" /> representing the location of the media resource.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when attempting to assign a null value.
    /// </exception>
    public required Uri Uri { get; set; }

    /// <summary>
    ///     Gets or sets the size of the media in bytes.
    /// </summary>
    /// <value>
    ///     A <see cref="long" /> representing the size of the media content in bytes.
    /// </value>
    /// <exception cref="System.OverflowException">
    ///     Thrown when the size value exceeds the limits of a <see cref="long" /> data type.
    /// </exception>
    public long Size { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the media was created.
    /// </summary>
    /// <remarks>
    ///     This property represents the timestamp at which the associated media item was created.
    /// </remarks>
    /// <value>
    ///     A <see cref="DateTime" /> value indicating the creation time of the media.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the assigned value is outside the valid range for <see cref="DateTime" />.
    /// </exception>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the type of media content.
    /// </summary>
    /// <value>
    ///     An enumeration of type <c>MediaType</c> representing the type of media content.
    ///     Possible values include Undefined, Video, Image, and File.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to set an invalid MediaType.
    /// </exception>
    public MediaType MediaType { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    /// <summary>
    ///     Represents a collection of metadata associated with the media resource.
    /// </summary>
    /// <remarks>
    ///     Metadata is stored as key-value pairs where both the key and value are strings.
    ///     The value can also be null, indicating the absence of a specific piece of information.
    /// </remarks>
    /// <value>
    ///     A dictionary containing metadata about the media file.
    /// </value>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown when attempting to access or modify the metadata without initializing the dictionary.
    /// </exception>
    public Dictionary<string, string?> Metadata { get; set; } = [];
}