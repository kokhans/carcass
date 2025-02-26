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
using Carcass.Yaml.YamlDotNet.Providers.Abstracts;
using YamlDotNet.Serialization;

// ReSharper disable ReturnTypeCanBeNotNullable

namespace Carcass.Yaml.YamlDotNet.Providers;

/// <summary>
///     Provides functionality for YAML serialization and deserialization using the YamlDotNet library.
///     Implements the <see cref="IYamlDotNetProvider" /> interface.
/// </summary>
public sealed class YamlDotNetProvider : IYamlDotNetProvider
{
    /// <summary>
    ///     Represents the deserialization functionality for YAML data.
    ///     Used to convert YAML strings into C# objects.
    /// </summary>
    private readonly IDeserializer _deserializer;

    /// <summary>
    ///     Represents the serializer instance used to convert objects into
    ///     their YAML string representation within the context of the provider.
    /// </summary>
    /// <remarks>
    ///     This field is utilized internally for serialization operations, leveraging
    ///     the YamlDotNet library's implementation of ISerializer.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown during the provider's instantiation if the serializer dependency is null.
    /// </exception>
    private readonly ISerializer _serializer;

    /// <summary>
    ///     Provides functionality for serializing and deserializing YAML data using the YamlDotNet library.
    /// </summary>
    public YamlDotNetProvider(ISerializer serializer, IDeserializer deserializer)
    {
        ArgumentVerifier.NotNull(serializer, nameof(serializer));
        ArgumentVerifier.NotNull(deserializer, nameof(deserializer));

        _serializer = serializer;
        _deserializer = deserializer;
    }

    /// <summary>
    ///     Attempts to deserialize a YAML string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to. Must be a reference type.</typeparam>
    /// <param name="yaml">The YAML string to deserialize.</param>
    /// <returns>The deserialized object of type <typeparamref name="T" />, or null if deserialization fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="yaml" /> parameter is null.</exception>
    public T? TryDeserialize<T>(string yaml) where T : class
    {
        ArgumentVerifier.NotNull(yaml, nameof(yaml));

        return _deserializer.Deserialize<T>(yaml);
    }

    /// <summary>
    ///     Attempts to deserialize a YAML string into an object of the specified type.
    /// </summary>
    /// <param name="yaml">The YAML string to be deserialized.</param>
    /// <param name="type">The target type to deserialize into.</param>
    /// <returns>
    ///     An instance of the specified type if deserialization is successful; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="yaml" /> parameter is null.</exception>
    public object? TryDeserialize(string yaml, Type type)
    {
        ArgumentVerifier.NotNull(yaml, nameof(yaml));

        return _deserializer.Deserialize(yaml, type);
    }

    /// <summary>
    ///     Deserializes a YAML string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize the YAML string into.</typeparam>
    /// <param name="yaml">The YAML string to be deserialized. Must not be null or empty.</param>
    /// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="yaml" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the deserialized data is null.</exception>
    public T Deserialize<T>(string yaml) where T : class
    {
        ArgumentVerifier.NotNull(yaml, nameof(yaml));

        return TryDeserialize<T>(yaml) ?? throw new InvalidOperationException("Data is null.");
    }

    /// <summary>
    ///     Deserializes a YAML-formatted string into an object of the specified type.
    /// </summary>
    /// <param name="yaml">The YAML string to deserialize. Cannot be null or empty.</param>
    /// <param name="type">The target type to which the YAML string will be deserialized. Cannot be null.</param>
    /// <returns>An object of the specified type representing the deserialized YAML string.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="yaml" /> or <paramref name="type" /> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the deserialization process does not produce a valid object.
    /// </exception>
    public object Deserialize(string yaml, Type type)
    {
        ArgumentVerifier.NotNull(yaml, nameof(yaml));

        return TryDeserialize(yaml, type) ?? throw new InvalidOperationException("Data is null.");
    }

    /// <summary>
    ///     Serializes an object of type <typeparamref name="T" /> into a YAML-formatted string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize. The type must be a reference type.</typeparam>
    /// <param name="data">The object to serialize. It must not be null.</param>
    /// <returns>A string containing the YAML representation of the object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="data" /> is null.</exception>
    public string Serialize<T>(T data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        return _serializer.Serialize(data);
    }
}