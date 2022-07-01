// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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

namespace Carcass.Core.Helpers;

public static class AssemblyHelper
{
    public static ReadOnlyCollection<Assembly> GetLoadedAssemblies(string searchPattern = "*.dll") =>
        Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern)
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            .ToList()
            .AsReadOnlyCollection();

    public static ReadOnlyCollection<TypeInfo>? GetTypeInfosFromLoadedAssemblies(
        IList<Assembly>? source,
        Func<TypeInfo, bool>? filter = default
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

    public static TypeInfo? GetTypeInfoFromLoadedAssemblies(
        IList<Assembly>? source,
        string? value,
        Func<TypeInfo, bool>? filter = default
    )
    {
        if (source is null || !source.Any() || string.IsNullOrWhiteSpace(value))
            return null;

        return GetTypeInfosFromLoadedAssemblies(source, filter)?
            .SingleOrDefault(ti =>
                ti.FullName is not null && ti.FullName.Equals(value, StringComparison.InvariantCultureIgnoreCase)
            );
    }

    public static TypeInfo? GetTypeInfoFromExecutionAssembly(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : Type.GetType(value)?.GetTypeInfo();

    public static string? GetDescription(Type? value)
    {
        if (value is null)
            return null;

        DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[]) value
            .GetCustomAttributes(typeof(DescriptionAttribute), false);

        return descriptionAttributes.Length == 0 ? null : descriptionAttributes[0].Description;
    }

    public static bool? HasAttribute<T>(this Type? value) where T : Attribute
    {
        if (value is null)
            return null;

        return Attribute.GetCustomAttribute(value, typeof(T)) is not null;
    }

    public static TAttribute? GetAttribute<TAttribute>(this Type? value) where TAttribute : Attribute
    {
        if (value is null)
            return null;

        return Attribute.GetCustomAttribute(value, typeof(TAttribute)) as TAttribute;
    }

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