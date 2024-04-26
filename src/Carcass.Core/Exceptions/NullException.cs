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

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

namespace Carcass.Core.Exceptions;

/// <summary>
///     Represents an exception that is thrown when an unexpected null value is encountered,
///     or additional metadata needs to be supplied for context regarding the null value.
/// </summary>
public class NullException : Exception
{
    /// <summary>
    ///     Represents an exception that is thrown when a null-related issue occurs.
    ///     Provides additional constructors for specific use cases, including support for metadata.
    /// </summary>
    public NullException()
    {
    }

    /// <summary>
    ///     Represents an exception that is thrown when a null value is encountered where it is not expected.
    /// </summary>
    public NullException(string? message) : base(message)
    {
    }

    /// <summary>
    ///     Represents an exception that is thrown when a null value is encountered where it is not expected.
    /// </summary>
    public NullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Represents an exception that is thrown when a null value is encountered in a context where it is not allowed.
    /// </summary>
    public NullException(Dictionary<string, object?> metadata)
    {
        ArgumentVerifier.NotNull(metadata, nameof(metadata));

        Metadata = metadata.AsReadOnlyDictionary();
    }

    /// <summary>
    ///     Represents metadata associated with an exception.
    ///     Provides additional contextual information in a key-value pair structure.
    /// </summary>
    /// <remarks>
    ///     The metadata is represented as a read-only dictionary, ensuring immutability
    ///     once it is initialized.
    /// </remarks>
    public ReadOnlyDictionary<string, object?>? Metadata { get; }
}