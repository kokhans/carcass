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

namespace Carcass.Core;

/// <summary>
///     Represents a placeholder type indicating the absence of a value or result.
/// </summary>
public record struct Nothing
{
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    /// <summary>
    ///     Represents a static instance of a <see cref="Nothing" /> value.
    ///     Provides a shared, immutable instance to indicate a "nothing" or "void-like" result.
    /// </summary>
    /// <remarks>
    ///     This property is useful in scenarios where a "no-operation" or absence of meaningful data is required.
    /// </remarks>
    /// <returns>
    ///     Returns a shared instance of the <see cref="Nothing" /> struct.
    /// </returns>
    public static Nothing None { get; }

    /// <summary>
    ///     Converts the instance of the <see cref="Nothing" /> struct to its string representation.
    /// </summary>
    /// <returns>
    ///     A string "Nothing" that represents the current instance of the <see cref="Nothing" /> struct.
    /// </returns>
    public override string ToString() => "Nothing";
}