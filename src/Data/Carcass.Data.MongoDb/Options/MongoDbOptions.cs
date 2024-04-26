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

namespace Carcass.Data.MongoDb.Options;

/// <summary>
///     Represents the configuration options for MongoDB,
///     including the connection string and database name.
/// </summary>
public sealed class MongoDbOptions
{
    /// <summary>
    ///     Represents the connection string used to connect to the MongoDB server.
    /// </summary>
    /// <remarks>
    ///     This property is required and must contain a valid connection string
    ///     that specifies the necessary information for accessing the MongoDB instance,
    ///     including server address, port, and optional authentication credentials.
    /// </remarks>
    /// <exception cref="ValidationException">
    ///     Thrown if the connection string is not specified or is invalid.
    /// </exception>
    [Required]
    public required string ConnectionString { get; init; }

    /// <summary>
    ///     Gets the name of the MongoDB database to be used by the application.
    /// </summary>
    /// <value>
    ///     A required string representing the name of the MongoDB database.
    /// </value>
    /// <exception cref="ValidationException">
    ///     Thrown when the database name is not provided during configuration.
    /// </exception>
    [Required]
    public required string DatabaseName { get; init; }
}