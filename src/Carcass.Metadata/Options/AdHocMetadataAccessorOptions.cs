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

using Carcass.Metadata.Accessors.AdHoc;

namespace Carcass.Metadata.Options;

/// <summary>
///     Represents a class for configuring options related to ad-hoc metadata resolution.
/// </summary>
public sealed class AdHocMetadataAccessorOptions
{
    /// <summary>
    ///     Defines the strategy to be used when resolving conflicts between persisted metadata and ad hoc metadata.
    /// </summary>
    /// <value>
    ///     The <see cref="AdHocMetadataResolutionStrategy" /> enumeration value that determines how metadata conflicts
    ///     are handled during resolution. This property must be set to one of the predefined strategies such as
    ///     <see cref="AdHocMetadataResolutionStrategy.ReplaceWithAdHoc" />,
    ///     <see cref="AdHocMetadataResolutionStrategy.LeavePersisted" />,
    ///     or <see cref="AdHocMetadataResolutionStrategy.ThrowsException" />.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown during metadata resolution when the value is set to
    ///     <see cref="AdHocMetadataResolutionStrategy.ThrowsException" />
    ///     and a metadata conflict occurs.
    /// </exception>
    public AdHocMetadataResolutionStrategy ResolutionStrategy { get; set; }
}