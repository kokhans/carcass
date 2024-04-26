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

using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// ReSharper disable ClassNeverInstantiated.Global

namespace Carcass.Swashbuckle.Exclude;

/// <summary>
///     A filter for Swashbuckle that modifies OpenAPI schema definitions by excluding properties
///     annotated with the <see cref="SwashbuckleExcludeAttribute" />.
/// </summary>
public sealed class SwashbuckleExcludeSchemaFilter : ISchemaFilter
{
    /// <summary>
    ///     Modifies the schema in the OpenAPI documentation by removing properties
    ///     marked with the <see cref="SwashbuckleExcludeAttribute" /> from the schema definition.
    /// </summary>
    /// <param name="schema">The schema to modify. Can be null.</param>
    /// <param name="context">
    ///     The context containing information about the schema being processed,
    ///     including the type for which the schema is being generated.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="context" /> or its <see cref="SchemaFilterContext.Type" />
    ///     property is null.
    /// </exception>
    public void Apply(OpenApiSchema? schema, SchemaFilterContext context)
    {
        if (schema?.Properties is null)
            return;

        List<PropertyInfo> propertyInfos = context.Type
            .GetProperties()
            .Where(pi => pi.GetCustomAttribute<SwashbuckleExcludeAttribute>() is not null)
            .ToList();

        foreach (string lowerCasePropertyInfoName in propertyInfos
                     .Select(pi => pi.Name.ToLower())
                     .Where(s => schema.Properties.ContainsKey(s)))
            schema.Properties.Remove(lowerCasePropertyInfoName);
    }
}