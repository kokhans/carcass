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

using System.ComponentModel;

namespace Carcass.Data.EntityFrameworkCore.Audit;

/// <summary>
///     Represents the types of JSON storage column formats supported within the database context.
/// </summary>
public enum JsonColumnType
{
    /// <summary>
    ///     Represents a JSON column type stored as "jsonb" in the database.
    /// </summary>
    /// <remarks>
    ///     This enum member is typically used to specify a column type that supports semi-structured
    ///     or structured data, allowing advanced query functionality with JSON data structures.
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     Thrown if this enum value is used in a scenario where only non-jsonb column types are supported.
    /// </exception>
    /// <returns>
    ///     A value corresponding to a PostgreSQL "jsonb" type.
    /// </returns>
    [Description("jsonb")] Jsonb = 1,

    /// <summary>
    ///     Represents the "nvarchar(max)" database column type for storing JSON data in a SQL Server database.
    /// </summary>
    /// <remarks>
    ///     This enum member is specifically intended for scenarios where JSON data needs to be stored in a
    ///     SQL Server "nvarchar(max)" column. It is an alternative to the "jsonb" type, which is supported
    ///     by PostgreSQL.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if this value is not handled properly during configuration or usage. Ensure its integration
    ///     is appropriately managed in the database context options.
    /// </exception>
    [Description("nvarchar(max)")] Nvarchar
}