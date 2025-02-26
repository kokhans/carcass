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
using Carcass.Data.EntityFrameworkCore.DbContexts.Abstracts;
using Carcass.Data.EntityFrameworkCore.Sessions;
using Carcass.Data.EntityFrameworkCore.Sessions.Abstracts;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for configuring services related to Entity Framework Core sessions
///     in an ASP.NET Core Dependency Injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Adds the Entity Framework Core session services to the specified IServiceCollection
    ///     for the specified DbContext type.
    /// </summary>
    /// <typeparam name="TDbContext">
    ///     The type of the DbContext to be used with the Entity Framework Core session.
    /// </typeparam>
    /// <param name="services">
    ///     The IServiceCollection to which the Entity Framework Core session services are added.
    /// </param>
    /// <returns>
    ///     The updated IServiceCollection with the Entity Framework Core session services registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassEntityFrameworkCoreSession<TDbContext>(
        this IServiceCollection services
    ) where TDbContext : EntityFrameworkCoreDbContext<TDbContext>
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services
            .AddScoped<IEntityFrameworkCoreSession, EntityFrameworkCoreSession<TDbContext>>();
    }
}