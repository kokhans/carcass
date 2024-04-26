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

namespace Carcass.Swashbuckle.Options;

/// <summary>
///     Represents configuration options for Swashbuckle integration.
/// </summary>
public sealed class SwashbuckleOptions
{
    /// <summary>
    ///     Specifies the name of the API or service to be displayed in the Swagger documentation.
    /// </summary>
    /// <exception cref="ValidationException">
    ///     Thrown if the value assigned to this property is null or empty, as it is required.
    /// </exception>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     Specifies the version of the API documentation.
    /// </summary>
    /// <remarks>
    ///     This property is required and serves to define the version information of the Swagger/OpenAPI documentation.
    ///     It is used to differentiate between different versions of the API in generated documentation files.
    /// </remarks>
    /// <exception cref="ValidationException">
    ///     Thrown when the value is not assigned or is provided in an invalid format.
    /// </exception>
    [Required]
    public required string Version { get; init; }
}