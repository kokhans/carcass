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

using System.Collections.ObjectModel;
using Carcass.Core.Extensions;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Carcass.Core.Exceptions;

/// <summary>
///     Represents an exception that is thrown when a null argument or value is encountered where it is not allowed.
/// </summary>
public class NotNullException : Exception
{
    /// <summary>
    ///     Represents an exception that is thrown when a null argument is provided where non-null is expected.
    /// </summary>
    public NotNullException()
    {
    }

    /// <summary>
    ///     Represents an exception that is thrown when a null value is encountered,
    ///     where null values are not allowed.
    /// </summary>
    public NotNullException(string? message) : base(message)
    {
    }

    /// <summary>
    ///     Represents an exception that is thrown when a null value is encountered where it is not allowed.
    /// </summary>
    /// <remarks>
    ///     This exception can include additional metadata to provide context about the error.
    /// </remarks>
    public NotNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Represents an exception that is thrown when a null value is encountered where it is not allowed.
    /// </summary>
    public NotNullException(Dictionary<string, object?> metadata)
    {
        ArgumentVerifier.NotNull(metadata, nameof(metadata));

        Metadata = metadata.AsReadOnlyDictionary();
    }

    /// <summary>
    ///     Represents metadata as a read-only dictionary containing key-value pairs.
    ///     This property provides additional contextual information related to the exception.
    /// </summary>
    /// <value>
    ///     A read-only dictionary of metadata where each key is a string and the value is an object.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the metadata is accessed when it has not been initialized.
    /// </exception>
    public ReadOnlyDictionary<string, object?>? Metadata { get; }
}