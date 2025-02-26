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

namespace Carcass.Data.Core.EventSourcing.Checkpoints.Abstracts;

/// <summary>
///     Represents a checkpoint that tracks the state of a stream and its committed position.
/// </summary>
public interface ICheckpoint
{
    // ReSharper disable once UnusedMemberInSuper.Global
    /// <summary>
    ///     Gets or sets the name of the stream associated with the checkpoint.
    /// </summary>
    /// <value>
    ///     A string representing the name of the stream. This is used to identify the stream for which the checkpoint is
    ///     maintained.
    /// </value>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown when attempting to set this property to null.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     Thrown when the value provided is an empty or whitespace string.
    /// </exception>
    string StreamName { get; set; }

    // ReSharper disable once UnusedMemberInSuper.Global
    /// <summary>
    ///     Gets or sets the position in the stream that has been committed.
    /// </summary>
    /// <value>
    ///     The position of the last processed event in the stream.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an invalid position value is assigned.
    /// </exception>
    long CommittedPosition { get; set; }
}