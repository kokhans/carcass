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

namespace Carcass.Metadata.Accessors.AdHoc;

/// <summary>
///     Specifies the strategies for resolving conflicts between ad hoc metadata and persisted metadata.
/// </summary>
public enum AdHocMetadataResolutionStrategy
{
    /// <summary>
    ///     Specifies a strategy where any conflicts between persisted metadata and ad hoc metadata
    ///     are resolved by replacing the persisted metadata with the ad hoc metadata.
    /// </summary>
    /// <remarks>
    ///     This strategy prioritizes ad hoc metadata entirely, disregarding any pre-existing persisted data.
    /// </remarks>
    ReplaceWithAdHoc = 1,

    /// <summary>
    ///     Specifies the strategy to retain the existing persisted metadata without applying ad-hoc metadata.
    /// </summary>
    /// <remarks>
    ///     When this strategy is selected, any ad-hoc metadata provided will be ignored, and the already persisted metadata
    ///     will remain unchanged.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the operation does not allow leaving persisted metadata unchanged due to conflicting business rules or
    ///     constraints.
    /// </exception>
    LeavePersisted,

    /// <summary>
    ///     Specifies a strategy where conflicts between persisted metadata and ad hoc metadata
    ///     result in an exception being thrown.
    /// </summary>
    /// <remarks>
    ///     This strategy enforces strict conflict resolution by failing when any discrepancy
    ///     between persisted metadata and ad hoc metadata is detected, ensuring no silent overrides occur.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when a conflict between persisted metadata and ad hoc metadata is encountered during resolution.
    /// </exception>
    ThrowsException
}