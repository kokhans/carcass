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

using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Core.Locators;
using Carcass.Data.EntityFrameworkCore.Audit;
using Carcass.Data.EntityFrameworkCore.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using Microsoft.EntityFrameworkCore;

// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Data.EntityFrameworkCore.DbContexts.Abstracts;

/// <summary>
///     Represents an abstract DbContext implementation with additional configurations
///     and behavior for Entity Framework Core. This class is designed to streamline
///     integration with common patterns such as auditing and entity configurations.
/// </summary>
/// <typeparam name="TDbContext">
///     The specific type of DbContext that inherits from this base class.
/// </typeparam>
public abstract class EntityFrameworkCoreDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    /// <summary>
    ///     Represents the options used to configure the current database context.
    /// </summary>
    /// <remarks>
    ///     This variable is essential for managing and applying database configuration settings.
    ///     It is set during the construction of the database context and provides access to the
    ///     configuration and extensions required while initializing or configuring the database context.
    /// </remarks>
    /// <typeparam name="TDbContext">The type of the DbContext being configured.</typeparam>
    private readonly DbContextOptions<TDbContext> _dbContextOptions;

    /// <summary>
    ///     Represents an abstract base class for an Entity Framework Core database context
    ///     with built-in support for audit logging and model configuration extensions.
    /// </summary>
    /// <typeparam name="TDbContext">The specific type of the DbContext that inherits this class.</typeparam>
    protected EntityFrameworkCoreDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
        _dbContextOptions = options;

        ChangeTracker.LazyLoadingEnabled = false;
    }

    /// <summary>
    ///     Configures the database context with additional options or behaviors.
    /// </summary>
    /// <param name="optionsBuilder">The builder used to configure the database context options.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when required services from the <c>ServiceProviderLocator</c> are not found
    ///     or when <c>AuditOptionsExtension</c> is null and auditing is required.
    /// </exception>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        AuditOptionsExtension? auditEntryOptionsExtension =
            optionsBuilder.Options.FindAuditOptionsExtension();

        if (auditEntryOptionsExtension is not null)
            optionsBuilder.AddInterceptors(new AuditInterceptor(
                ServiceProviderLocator.Current.GetRequiredService<TimeProvider>(),
                ServiceProviderLocator.Current.GetRequiredService<IUserIdAccessor>(),
                auditEntryOptionsExtension
            ));
    }

    /// <summary>
    ///     Configures the model for the context by defining entity relationships, conventions, and other configurations.
    /// </summary>
    /// <param name="modelBuilder">
    ///     An instance of <see cref="ModelBuilder" /> used to construct the model for the database
    ///     context.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="modelBuilder" /> parameter is null.</exception>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        AuditOptionsExtension? auditEntryOptionsExtension =
            _dbContextOptions.FindAuditOptionsExtension();

        if (auditEntryOptionsExtension is not null && auditEntryOptionsExtension.UseAuditEntry)
            modelBuilder.Entity<AuditEntry>(etb =>
            {
                AuditEntryConfiguration configuration = new(
                    ServiceProviderLocator.Current.GetRequiredService<IJsonProvider>(),
                    auditEntryOptionsExtension
                );
                configuration.Configure(etb);
            });

        modelBuilder.ApplyEntityConfigurations(GetType().Assembly);
        modelBuilder.ApplyIsDeletedQueryFilter();
    }
}