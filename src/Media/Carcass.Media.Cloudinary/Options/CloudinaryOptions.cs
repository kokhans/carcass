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

using System.ComponentModel.DataAnnotations;

namespace Carcass.Media.Cloudinary.Options;

/// <summary>
///     Represents the configuration options required for authentication and integration with a Cloudinary account.
///     This class is used to store essential credentials such as CloudName, ApiKey, and ApiSecret for Cloudinary access.
/// </summary>
public sealed class CloudinaryOptions
{
    /// <summary>
    ///     Represents the name of the cloud environment associated with the Cloudinary account.
    /// </summary>
    /// <remarks>
    ///     This property is required and must be provided to correctly configure the Cloudinary service.
    /// </remarks>
    /// <exception cref="ValidationException">
    ///     Thrown if the property value is not provided during configuration.
    /// </exception>
    [Required]
    public required string CloudName { get; init; }

    /// <summary>
    ///     Represents the required API key used for authenticating with the Cloudinary service.
    /// </summary>
    /// <exception cref="ValidationException">
    ///     Thrown when the property is not provided or contains invalid data.
    /// </exception>
    [Required]
    public required string ApiKey { get; init; }

    /// <summary>
    ///     Gets the API secret used for authentication in Cloudinary services.
    /// </summary>
    /// <remarks>
    ///     This property is required and must contain a valid API secret key provided
    ///     by the Cloudinary platform. It is used for secure integration with Cloudinary services.
    /// </remarks>
    /// <exception cref="ValidationException">
    ///     Throws when the value is not provided or invalid during validation.
    /// </exception>
    /// <value>
    ///     A string representing the API secret key for Cloudinary authentication.
    /// </value>
    [Required]
    public required string ApiSecret { get; init; }
}