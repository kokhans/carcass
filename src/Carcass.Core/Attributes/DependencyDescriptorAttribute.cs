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

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Carcass.Core.Attributes;

/// <summary>
///     Represents an attribute that provides metadata for a dependency descriptor.
///     This attribute can be applied to classes to define a descriptive name.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DependencyDescriptorAttribute : Attribute
{
    /// <summary>
    ///     An attribute used to provide a descriptor for a class dependency.
    ///     It is applied to a class to specify additional metadata about the dependency.
    /// </summary>
    public DependencyDescriptorAttribute(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name));

        Name = name;
    }

    /// <summary>
    ///     Gets the name associated with the dependency descriptor.
    /// </summary>
    /// <remarks>
    ///     This property provides the specified name for a class marked with the <see cref="DependencyDescriptorAttribute" />.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the name is null or an empty string when assigned during object initialization.
    /// </exception>
    /// <returns>
    ///     The name of the dependency descriptor as a string.
    /// </returns>
    public string Name { get; }
}