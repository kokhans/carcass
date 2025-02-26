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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using Carcass.Core.Extensions;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides utility methods for working with assemblies, types, and their attributes.
///     Useful for dynamically loading assemblies, retrieving type information, and extracting metadata.
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    ///     Retrieves all loaded assemblies from the current application domain that match the specified search pattern.
    /// </summary>
    /// <param name="searchPattern">
    ///     The search pattern to filter the assembly files. Defaults to "*.dll".
    /// </param>
    /// <returns>
    ///     A read-only collection of <see cref="System.Reflection.Assembly" /> that were loaded from files matching the search
    ///     pattern.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown if the <paramref name="searchPattern" /> is null.
    /// </exception>
    public static ReadOnlyCollection<Assembly> GetLoadedAssemblies(string searchPattern = "*.dll")
    {
        ArgumentVerifier.NotNull(searchPattern, nameof(searchPattern));

        return Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern)
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            .ToList()
            .AsReadOnlyCollection();
    }

    /// <summary>
    ///     Retrieves a collection of TypeInfo objects from a source list of assemblies, optionally filtered by a given
    ///     predicate.
    /// </summary>
    /// <param name="source">
    ///     A list of assemblies to retrieve TypeInfo objects from. Cannot be null or empty.
    /// </param>
    /// <param name="filter">
    ///     An optional predicate used to filter the TypeInfo objects. If null, no filtering will be applied.
    /// </param>
    /// <returns>
    ///     A read-only collection of TypeInfo objects matching the filter, or null if the source is null or empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the source is null or contains invalid elements.
    /// </exception>
    public static ReadOnlyCollection<TypeInfo>? GetTypeInfosFromLoadedAssemblies(
        IList<Assembly>? source,
        Func<TypeInfo, bool>? filter = null
    )
    {
        if (source is null || !source.Any())
            return null;

        ArgumentVerifier.NotNull(source, nameof(source));

        IEnumerable<TypeInfo> query = source.SelectMany(a => a.DefinedTypes);

        if (filter is not null)
            query = query.Where(filter);

        return query.ToList().AsReadOnlyCollection();
    }

    /// <summary>
    ///     Retrieves a specific <see cref="TypeInfo" /> from a collection of loaded assemblies based on the specified type
    ///     name.
    /// </summary>
    /// <param name="source">
    ///     The list of assemblies to search for the specified type. Can be null or empty, in which case the method returns
    ///     null.
    /// </param>
    /// <param name="value">
    ///     The fully qualified name of the type to find. Must not be null, empty, or consist solely of whitespace.
    ///     If the name is invalid, the method returns null.
    /// </param>
    /// <param name="filter">
    ///     An optional filter predicate to apply when retrieving the list of type information objects. If null, no filtering
    ///     is applied.
    /// </param>
    /// <returns>
    ///     A <see cref="TypeInfo" /> representing the type with the specified name, or null if no such type is found
    ///     or the input parameters are invalid.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="source" /> parameter is null.
    /// </exception>
    public static TypeInfo? GetTypeInfoFromLoadedAssemblies(
        IList<Assembly>? source,
        string? value,
        Func<TypeInfo, bool>? filter = null
    )
    {
        if (source is null || !source.Any() || string.IsNullOrWhiteSpace(value))
            return null;

        return GetTypeInfosFromLoadedAssemblies(source, filter)?
            .SingleOrDefault(ti =>
                ti.FullName is not null && ti.FullName.Equals(value, StringComparison.InvariantCultureIgnoreCase)
            );
    }

    /// <summary>
    ///     Retrieves type information for a specified type name from the currently executing assembly.
    /// </summary>
    /// <param name="value">The fully qualified name of the type to retrieve information for. Can be null or empty.</param>
    /// <returns>
    ///     A <see cref="TypeInfo" /> object representing the specified type if found; otherwise, null.
    /// </returns>
    /// <exception cref="TypeLoadException">
    ///     Thrown when the type specified by the given name cannot be found in the executing assembly.
    /// </exception>
    public static TypeInfo? GetTypeInfoFromExecutionAssembly(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : Type.GetType(value)?.GetTypeInfo();

    /// <summary>
    ///     Retrieves the description metadata from a specified <see cref="Type" /> if the type is decorated with a
    ///     <see cref="DescriptionAttribute" />.
    /// </summary>
    /// <param name="value">The <see cref="Type" /> object to inspect for a description attribute.</param>
    /// <returns>
    ///     The description string from the <see cref="DescriptionAttribute" />, or null if the attribute is missing or
    ///     the <paramref name="value" /> is null.
    /// </returns>
    /// <exception cref="AmbiguousMatchException">
    ///     Thrown if the specified <see cref="Type" /> contains multiple description
    ///     attributes, which is generally not expected.
    /// </exception>
    public static string? GetDescription(Type? value)
    {
        if (value is null)
            return null;

        DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[]) value
            .GetCustomAttributes(typeof(DescriptionAttribute), false);

        return descriptionAttributes.Length == 0 ? null : descriptionAttributes[0].Description;
    }

    /// <summary>
    ///     Determines whether the specified type has a custom attribute of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the attribute to check for.</typeparam>
    /// <param name="value">The type to inspect for the presence of the attribute.</param>
    /// <returns>
    ///     A nullable boolean indicating whether the specified attribute exists. Returns null if the provided type is null.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided type is null.</exception>
    public static bool? HasAttribute<T>(this Type? value) where T : Attribute
    {
        if (value is null)
            return null;

        return Attribute.GetCustomAttribute(value, typeof(T)) is not null;
    }

    /// <summary>
    ///     Retrieves a custom attribute of the specified type from the given type, if present.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to retrieve.</typeparam>
    /// <param name="value">The type from which to retrieve the attribute.</param>
    /// <returns>The attribute of the specified type if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="value" /> is null.</exception>
    public static TAttribute? GetAttribute<TAttribute>(this Type? value) where TAttribute : Attribute
    {
        if (value is null)
            return null;

        return Attribute.GetCustomAttribute(value, typeof(TAttribute)) as TAttribute;
    }

    /// <summary>
    ///     Retrieves the namespace of the calling class in the current execution stack.
    /// </summary>
    /// <returns>
    ///     The namespace of the calling class as a string, or null if the calling class or namespace cannot be determined.
    /// </returns>
    /// <exception cref="System.NullReferenceException">
    ///     Thrown if the method is unable to access the declaring type or method information in the current execution stack.
    /// </exception>
    public static string? GetCallingClassNamespace()
    {
        string? @namespace;
        Type? declaringType;
        int skipFrames = 2;
        do
        {
            MethodBase? method = new StackFrame(skipFrames, false).GetMethod();
            if (method is null)
                return null;

            declaringType = method.DeclaringType;
            if (declaringType is null)
                return method.Name;

            skipFrames++;
            @namespace = declaringType.Namespace;
        } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        return @namespace;
    }
}