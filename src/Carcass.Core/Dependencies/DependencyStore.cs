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

namespace Carcass.Core.Dependencies;

/// <summary>
///     A generic class that provides storage and management of dependencies keyed by their string names.
/// </summary>
/// <typeparam name="TDependency">
///     The type of dependency to store. Must be a reference type.
/// </typeparam>
public sealed class DependencyStore<TDependency>(Dictionary<string, TDependency>? dependencies = null)
    where TDependency : class
{
    /// <summary>
    ///     Stores dependencies in a dictionary and provides methods for managing and retrieving them.
    /// </summary>
    /// <typeparam name="TDependency">
    ///     The type of the dependencies to be stored. The type must be a reference type.
    /// </typeparam>
    /// <remarks>
    ///     This field is initialized with an optional provided dictionary or a new empty dictionary.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when any methods using this field are provided with null arguments.
    /// </exception>
    /// <returns>
    ///     The internally stored dictionary of dependencies.
    /// </returns>
    private readonly Dictionary<string, TDependency> _dependencies =
        dependencies ?? new Dictionary<string, TDependency>();

    /// <summary>
    ///     Adds a dependency to the internal store with the specified name as the key.
    /// </summary>
    /// <param name="name">The name of the dependency to add. It must not be null or empty.</param>
    /// <param name="dependency">The dependency instance to store. It must not be null.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="name" /> or <paramref name="dependency" /> is
    ///     null.
    /// </exception>
    public void AddDependency(string name, TDependency dependency)
    {
        ArgumentVerifier.NotNull(name, nameof(name));
        ArgumentVerifier.NotNull(dependency, nameof(dependency));

        _dependencies[name] = dependency;
    }

    /// <summary>
    ///     Retrieves a required dependency by its name from the dependency store.
    /// </summary>
    /// <param name="name">The name of the dependency to retrieve.</param>
    /// <returns>The dependency associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the dependency with the specified name does not exist in the store.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided name is null or empty.</exception>
    public TDependency GetRequiredDependency(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name));

        return _dependencies[name];
    }

    /// <summary>
    ///     Retrieves an optional dependency by its name from the internal storage.
    /// </summary>
    /// <param name="name">The name of the dependency to retrieve.</param>
    /// <returns>The dependency if it exists; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="name" /> parameter is <c>null</c> or empty.
    /// </exception>
    public TDependency? GetOptionalDependency(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name));

        _dependencies.TryGetValue(name, out TDependency? value);

        return value;
    }

    /// <summary>
    ///     Retrieves all dependencies stored in the dependency collection.
    /// </summary>
    /// <returns>
    ///     A dictionary containing all stored dependencies, where the keys are the dependency names
    ///     and the values are the corresponding dependency instances.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Throws if there is an unexpected state in the dependency collection access.
    /// </exception>
    public Dictionary<string, TDependency> GetDependencies() => _dependencies;
}