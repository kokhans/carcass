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

// ReSharper disable UnusedMember.Global

namespace Carcass.Json.Core.Providers.Abstracts;

/// <summary>
///     Provides functionality for serializing and deserializing JSON data.
/// </summary>
public interface IJsonProvider
{
    /// <summary>
    ///     Attempts to deserialize a JSON string into an object of the specified type.
    ///     Returns null if deserialization fails or the JSON is invalid.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize. Must be a class.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An instance of the specified type if deserialization succeeds; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="json" /> parameter is null.</exception>
    T? TryDeserialize<T>(string json) where T : class;

    /// <summary>
    ///     Tries to deserialize a JSON string into an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The target type to deserialize into.</param>
    /// <returns>
    ///     The deserialized object if successful, or null if deserialization fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="json" /> or <paramref name="type" /> is null.
    /// </exception>
    object? TryDeserialize(string json, Type type);

    /// <summary>
    ///     Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize. Must be a reference type.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of type <typeparamref name="T" /> created from the JSON string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="json" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the deserialized object is null or the JSON string is invalid.</exception>
    T Deserialize<T>(string json) where T : class;

    /// <summary>
    ///     Deserializes a JSON string into an object of a specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize. Must not be null or empty.</param>
    /// <param name="type">The target type into which the JSON string is deserialized. Must not be null.</param>
    /// <returns>An object of the specified type represented by the JSON string.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when either <paramref name="json" /> or <paramref name="type" /> is
    ///     null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the JSON cannot be deserialized into the specified type or
    ///     results in a null object.
    /// </exception>
    object Deserialize(string json, Type type);

    /// <summary>
    ///     Serializes the specified object into its JSON string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize. Must be a reference type.</typeparam>
    /// <param name="data">The object to serialize. Cannot be null.</param>
    /// <returns>A JSON string representation of the specified object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided object is null.</exception>
    string Serialize<T>(T data) where T : class;
}