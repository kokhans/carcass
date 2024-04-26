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

namespace Carcass.Media.Core.Models;

/// <summary>
///     Represents the type of media managed or processed in the system.
/// </summary>
/// <remarks>
///     This enumeration defines different categories of media, such as
///     undefined, video, image, or file, aiding in classification
///     and handling within the application.
/// </remarks>
/// <exception cref="ArgumentException">
///     Thrown when an operation is performed using an invalid or unsupported media type.
/// </exception>
/// <returns>
///     An enumeration value of <see cref="MediaType" /> that specifies the category of media.
/// </returns>
public enum MediaType
{
    /// <summary>
    ///     Represents an undefined media type.
    /// </summary>
    /// <remarks>
    ///     Typically used as a fallback or for uninitialized media type values,
    ///     it denotes that the media type is not specified or cannot be determined.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to perform an operation requiring a specific media type
    ///     while it is set to Undefined.
    /// </exception>
    /// <returns>
    ///     The <see cref="MediaType" /> enumeration value indicating an undefined type.
    /// </returns>
    Undefined = 1,

    /// <summary>
    ///     Represents a media type categorized as a video in the system.
    /// </summary>
    /// <remarks>
    ///     This media type is used for media items that are identified as videos,
    ///     encompassing formats specifically designed for or used in video content.
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     Thrown when an invalid operation is performed with this media type.
    /// </exception>
    /// <returns>
    ///     The <see cref="MediaType" /> enumeration value that signifies the video type.
    /// </returns>
    Video,

    /// <summary>
    ///     Represents a media type categorized as an image.
    /// </summary>
    /// <remarks>
    ///     This media type is used for media items that are image files, such as photographs,
    ///     graphics, or any visual representation in image format.
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     Thrown when an invalid operation is performed with this media type.
    /// </exception>
    /// <returns>
    ///     The <see cref="MediaType" /> enumeration value that indicates the image type.
    /// </returns>
    Image,

    /// <summary>
    ///     Represents a media type categorized as a file.
    /// </summary>
    /// <remarks>
    ///     This media type is designated for media items stored as generic files, which
    ///     do not conform to specific types such as video or image.
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     Thrown if operations on this media type are incompatible or improperly handled.
    /// </exception>
    /// <returns>
    ///     The <see cref="MediaType" /> enumeration value indicating that the item is a file.
    /// </returns>
    File
}