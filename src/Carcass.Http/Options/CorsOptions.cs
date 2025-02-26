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

#pragma warning disable CS8618

namespace Carcass.Http.Options;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Represents the configuration options for Cross-Origin Resource Sharing (CORS).
/// </summary>
public sealed class CorsOptions
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Gets or sets the list of allowed origins for cross-origin resource sharing (CORS).
    /// </summary>
    /// <remarks>
    ///     This property specifies the origins that are permitted to access the application's resources.
    /// </remarks>
    /// <value>An array of strings representing the allowed origins.</value>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
    ///     Thrown if the property is not set or contains invalid entries.
    /// </exception>
    [Required]
    public string[] AllowedOrigins { get; set; }
}