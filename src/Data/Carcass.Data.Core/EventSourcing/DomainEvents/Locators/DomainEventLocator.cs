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

using System.Text.RegularExpressions;
using Carcass.Core;
using Carcass.Core.Helpers;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;

namespace Carcass.Data.Core.EventSourcing.DomainEvents.Locators;

/// <summary>
///     Provides functionality to locate and retrieve domain event types dynamically within the application.
/// </summary>
public sealed partial class DomainEventLocator : IDomainEventLocator
{
    /// <summary>
    ///     A dictionary that maps domain event names (in lowercase) to their corresponding <see cref="System.Type" />.
    ///     This mapping facilitates quick lookup of domain event types based on their names.
    /// </summary>
    /// <remarks>
    ///     The keys in the dictionary are the lowercase versions of domain event names, and
    ///     the values are the associated <see cref="System.Type" /> instances representing those domain events.
    ///     The dictionary is populated during the initialization phase using reflection to identify
    ///     domain event types from loaded assemblies that match specific naming patterns.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    ///     Thrown during the initialization process if duplicate domain event names are detected
    ///     in the mapped assemblies.
    /// </exception>
    private readonly Dictionary<string, Type> _domainEventsMap;

    /// <summary>
    ///     Provides functionality to locate and retrieve domain event types by their names.
    /// </summary>
    public DomainEventLocator()
    {
        _domainEventsMap = AssemblyHelper.GetLoadedAssemblies("*.Domain.dll")
            .SelectMany(a => a.DefinedTypes
                .Where(t => DomainEventNameRegex().IsMatch(t.Name))
            )
            .Select(t => t.AsType())
            .ToDictionary(t => t.Name.ToLower());
    }

    /// <summary>
    ///     Retrieves the <see cref="Type" /> of a domain event based on its name.
    /// </summary>
    /// <param name="domainEventName">The name of the domain event used to find the corresponding type.</param>
    /// <returns>
    ///     The <see cref="Type" /> of the domain event if it is found in the
    ///     internal domain events map; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="domainEventName" /> is null.
    /// </exception>
    public Type? GetDomainEventType(string domainEventName)
    {
        ArgumentVerifier.NotNull(domainEventName, nameof(domainEventName));

        string lowercasedEventName = domainEventName.ToLower();

        return _domainEventsMap.GetValueOrDefault(lowercasedEventName);
    }

    /// <summary>
    ///     Compiles a regular expression to identify domain event class names within an assembly.
    ///     The regex matches names that end with "DomainEvent".
    /// </summary>
    /// <returns>
    ///     A <see cref="System.Text.RegularExpressions.Regex" /> instance to match domain event names.
    /// </returns>
    [GeneratedRegex(@"\w*DomainEvent\b")]
    private static partial Regex DomainEventNameRegex();
}