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

using Carcass.Core;
using Carcass.Json.NewtonsoftJson.Providers.Abstracts;
using Newtonsoft.Json;

namespace Carcass.Json.NewtonsoftJson.Providers;

/// <summary>
///     Provides functionality for serializing and deserializing JSON data using the Newtonsoft.Json library.
/// </summary>
public sealed class NewtonsoftJsonProvider : INewtonsoftJsonProvider
{
    /// <summary>
    ///     Represents a JSON serializer instance configured with specific settings for
    ///     handling serialization and deserialization tasks.
    /// </summary>
    /// <remarks>
    ///     This member is initialized using the <see cref="JsonSerializerSettings" /> provided
    ///     during the construction of the <c>NewtonsoftJsonProvider</c>. The serializer is used internally
    ///     for converting objects to their JSON representations and vice versa.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the serialization or deserialization process encounters errors.
    /// </exception>
    private readonly JsonSerializer _jsonSerializer;

    /// <summary>
    ///     Represents the settings used by the Newtonsoft.Json serialization and deserialization processes
    ///     within the <see cref="NewtonsoftJsonProvider" />.
    /// </summary>
    /// <remarks>
    ///     The settings define behavior such as formatting, converters, contract resolvers, and other
    ///     customizations specific to the Newtonsoft.Json library.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided settings are null during the initialization of the <see cref="NewtonsoftJsonProvider" />.
    /// </exception>
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    /// <summary>
    ///     Provides JSON functionality using the Newtonsoft.Json library,
    ///     including serialization and deserialization with customizable settings.
    /// </summary>
    public NewtonsoftJsonProvider(JsonSerializerSettings jsonSerializerSettings)
    {
        ArgumentVerifier.NotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));

        _jsonSerializerSettings = jsonSerializerSettings;
        _jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
    }

    /// <summary>
    ///     Attempts to deserialize the given JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the JSON string into. Must be a class.</typeparam>
    /// <param name="json">The JSON string to deserialize. Must not be null.</param>
    /// <returns>
    ///     The deserialized object of type <typeparamref name="T" /> if the operation is successful; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided JSON string is null.</exception>
    public T? TryDeserialize<T>(string json) where T : class
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
    }

    /// <summary>
    ///     Attempts to deserialize a JSON string into an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to be deserialized.</param>
    /// <param name="type">The target type into which the JSON string will be deserialized.</param>
    /// <returns>The deserialized object if successful; otherwise, null if deserialization fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json" /> or <paramref name="type" /> is null.</exception>
    public object? TryDeserialize(string json, Type type)
    {
        ArgumentVerifier.NotNull(json, nameof(json));
        ArgumentVerifier.NotNull(type, nameof(type));

        return JsonConvert.DeserializeObject(json, type, _jsonSerializerSettings);
    }

    /// <summary>
    ///     Deserializes a JSON string into an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize into. Must be a reference type.</typeparam>
    /// <param name="json">The JSON string to deserialize. Cannot be null.</param>
    /// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if deserialization results in a null object.</exception>
    public T Deserialize<T>(string json) where T : class
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return TryDeserialize<T>(json) ?? throw new InvalidOperationException("Data is null.");
    }

    /// <summary>
    ///     Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize. It cannot be null.</param>
    /// <param name="type">The target type to deserialize the JSON content into. It cannot be null.</param>
    /// <returns>The deserialized object of the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided JSON string or type is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the deserialization process results in a null object.</exception>
    public object Deserialize(string json, Type type)
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return TryDeserialize(json, type) ?? throw new InvalidOperationException("Data is null.");
    }

    /// <summary>
    ///     Serializes the specified object to a JSON-formatted string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize. Must be a class.</typeparam>
    /// <param name="data">The object to serialize. Cannot be null.</param>
    /// <returns>A JSON-formatted string representation of the object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="data" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the resulting JSON string is null.</exception>
    public string Serialize<T>(T data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        TextWriter textWriter = new StringWriter();
        _jsonSerializer.Serialize(textWriter, data);

        string? json = textWriter.ToString();

        return json ?? throw new InvalidOperationException("JSON is null.");
    }
}