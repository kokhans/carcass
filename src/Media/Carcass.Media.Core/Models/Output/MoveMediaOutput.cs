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

using Carcass.Media.Core.Models.Output.Abstracts;

namespace Carcass.Media.Core.Models.Output;

/// <summary>
///     Represents the output of a media move operation, providing details about the moved media item
///     such as its location, type, and metadata.
/// </summary>
public sealed class MoveMediaOutput : MediaOutput
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    /// <summary>
    ///     Gets or sets the MIME type of the media content.
    /// </summary>
    /// <value>
    ///     A string representing the content type (e.g., "image/jpeg", "application/pdf") of the media.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the value being set is null.
    /// </exception>
    /// <remarks>
    ///     This property is used to indicate the type of the content, enabling proper handling and identification of the
    ///     media.
    /// </remarks>
    public required string ContentType { get; set; }
}