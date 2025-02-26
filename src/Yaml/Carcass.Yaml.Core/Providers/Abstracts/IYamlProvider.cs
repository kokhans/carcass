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

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Carcass.Yaml.Core.Providers.Abstracts;

/// <summary>
///     Defines methods for YAML serialization and deserialization.
/// </summary>
public interface IYamlProvider
{
    /// <summary>
    ///     Attempts to deserialize the given YAML string into an object of the specified type.
    ///     Returns null if deserialization fails or the input is invalid.
    /// </summary>
    /// <typeparam name="T">The type into which the YAML string should be deserialized. Must be a reference type.</typeparam>
    /// <param name="yaml">The YAML string to deserialize.</param>
    /// <returns>
    ///     An object of type <typeparamref name="T" /> that represents the deserialized data,
    ///     or null if the deserialization fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="yaml" /> is null.</exception>
    T? TryDeserialize<T>(string yaml) where T : class;

    /// <summary>
    ///     Attempts to deserialize a YAML string into an object of the specified type.
    /// </summary>
    /// <param name="yaml">The YAML string to be deserialized.</param>
    /// <param name="type">The target type for deserialization.</param>
    /// <returns>The deserialized object if successful; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="yaml" /> or <paramref name="type" /> is null.</exception>
    object? TryDeserialize(string yaml, Type type);

    /// <summary>
    ///     Deserializes a YAML string into an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the YAML string into. Must be a reference type.</typeparam>
    /// <param name="yaml">The YAML string to deserialize.</param>
    /// <returns>An object of type <typeparamref name="T" /> populated with data from the YAML string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="yaml" /> argument is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when deserialization fails or results in a null object.</exception>
    T Deserialize<T>(string yaml) where T : class;

    /// <summary>
    ///     Deserializes a YAML string into an object of the specified type.
    /// </summary>
    /// <param name="yaml">The YAML string to deserialize. Must not be null.</param>
    /// <param name="type">The type of the object to deserialize into. Must not be null.</param>
    /// <returns>An instance of the specified type populated with the data from the YAML string.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="yaml" /> or <paramref name="type" /> argument
    ///     is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown when the deserialization process produces a null result.</exception>
    object Deserialize(string yaml, Type type);

    /// <summary>
    ///     Serializes the given object into its YAML string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize, which must be a reference type.</typeparam>
    /// <param name="data">The object to serialize into YAML format. Must not be null.</param>
    /// <returns>The YAML string representation of the given object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="data" /> is null.</exception>
    string Serialize<T>(T data) where T : class;
}