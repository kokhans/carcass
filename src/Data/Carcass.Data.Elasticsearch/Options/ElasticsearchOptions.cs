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

namespace Carcass.Data.Elasticsearch.Options;

/// <summary>
///     Represents configuration options for integrating with Elasticsearch.
/// </summary>
public sealed class ElasticsearchOptions
{
    /// <summary>
    ///     Gets or initializes the URI used to connect to the Elasticsearch instance.
    /// </summary>
    /// <exception cref="ValidationException">
    ///     Thrown when the value is not provided or is invalid according to the data annotations.
    /// </exception>
    [Required]
    public required Uri Uri { get; init; }

    /// <summary>
    ///     Represents the audit configuration options for Elasticsearch.
    /// </summary>
    /// <remarks>
    ///     This property specifies the settings for indexing audit-related information in Elasticsearch.
    ///     It allows defining audit-specific configurations such as the target index.
    /// </remarks>
    /// <value>
    ///     An instance of <see cref="ElasticsearchAuditOptions" /> representing the audit configuration, or <c>null</c> if not
    ///     configured.
    /// </value>
    /// <exception cref="ValidationException">
    ///     Thrown if the target audit options or required fields within the audit configuration are not properly initialized.
    /// </exception>
    public ElasticsearchAuditOptions? Audit { get; init; }

    /// <summary>
    ///     Represents the configuration options for Elasticsearch auditing functionality.
    /// </summary>
    public sealed class ElasticsearchAuditOptions
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        /// <summary>
        ///     Represents the name of the Elasticsearch index used for storing audit entries.
        /// </summary>
        /// <exception cref="ValidationException">
        ///     Thrown when the property is not initialized or contains an invalid value.
        /// </exception>
        /// <returns>
        ///     A string representing the name of the Elasticsearch index.
        /// </returns>
        [Required]
        public required string Index { get; init; }
    }
}