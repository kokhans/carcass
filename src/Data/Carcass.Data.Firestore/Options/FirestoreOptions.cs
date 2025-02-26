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

namespace Carcass.Data.Firestore.Options;

/// <summary>
///     Represents the configuration options required to connect to a Firestore database.
/// </summary>
public sealed class FirestoreOptions
{
    /// <summary>
    ///     Gets or sets the unique identifier for the Google Firestore project.
    /// </summary>
    /// <remarks>
    ///     This property is required and must be set to the correct Project ID
    ///     associated with your Firestore instance.
    /// </remarks>
    /// <exception cref="ValidationException">
    ///     Thrown if the value is null, empty, or not provided during configuration.
    /// </exception>
    [Required]
    public required string ProjectId { get; init; }

    /// <summary>
    ///     Specifies the JSON credentials for accessing Google Firestore.
    ///     This property is required and should contain the necessary authentication details
    ///     formatted as a JSON string.
    /// </summary>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
    ///     Thrown when the property is null or empty, as it is marked as required.
    /// </exception>
    [Required]
    public required string Json { get; init; }
}