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

using System.Text.Json;
using Carcass.Core;
using Carcass.Json.SystemTextJson.Providers.Abstracts;

namespace Carcass.Json.SystemTextJson.Providers;

/// <summary>
///     Provides JSON serialization and deserialization functionalities using System.Text.Json.
///     Implements the <see cref="ISystemTextJsonProvider" /> interface.
/// </summary>
public sealed class SystemTextJsonProvider : ISystemTextJsonProvider
{
    /// <summary>
    ///     Represents the JSON serialization and deserialization options used throughout the provider.
    /// </summary>
    /// <remarks>
    ///     This variable stores configuration settings for JSON serialization and deserialization,
    ///     such as property naming policies, converters, and other behavior settings.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the value assigned to this variable is null during initialization.
    /// </exception>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    ///     Provides JSON serialization and deserialization functionality using System.Text.Json.
    /// </summary>
    public SystemTextJsonProvider(JsonSerializerOptions jsonSerializerOptions)
    {
        ArgumentVerifier.NotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));

        _jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    ///     Attempts to deserialize the provided JSON string into an object of the specified type.
    ///     Returns null if deserialization fails or the JSON represents a null value.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the JSON string into. Must be a reference type.</typeparam>
    /// <param name="json">The JSON string to be deserialized.</param>
    /// <returns>
    ///     An instance of type <typeparamref name="T" /> if deserialization is successful; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="json" /> parameter is null.
    /// </exception>
    public T? TryDeserialize<T>(string json) where T : class
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
    }

    /// <summary>
    ///     Attempts to deserialize a JSON string into an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize into.</param>
    /// <returns>
    ///     Returns an object of the specified type, or null if deserialization fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="json" /> or <paramref name="type" /> parameter is null.
    /// </exception>
    public object? TryDeserialize(string json, Type type)
    {
        ArgumentVerifier.NotNull(json, nameof(json));
        ArgumentVerifier.NotNull(type, nameof(type));

        return JsonSerializer.Deserialize(json, type, _jsonSerializerOptions);
    }

    /// <summary>
    ///     Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize. It must be a reference type.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="json" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the deserialization yields a null object.</exception>
    public T Deserialize<T>(string json) where T : class
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return TryDeserialize<T>(json) ?? throw new InvalidOperationException("Data is null.");
    }

    /// <summary>
    ///     Deserializes the specified JSON string into an object of the given type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize into.</param>
    /// <returns>An object of the specified type populated with the data from the JSON string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="json" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if deserialization results in a null object.</exception>
    public object Deserialize(string json, Type type)
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return TryDeserialize(json, type) ?? throw new InvalidOperationException("Data is null.");
    }

    /// <summary>
    ///     Serializes an object to a JSON-formatted string using the configured serialization options.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize. It must be a reference type.</typeparam>
    /// <param name="data">The object to serialize. Must not be null.</param>
    /// <returns>A JSON-formatted string representing the serialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="data" /> is null.</exception>
    public string Serialize<T>(T data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        return JsonSerializer.Serialize(data, _jsonSerializerOptions);
    }
}