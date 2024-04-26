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
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Carcass.Data.EntityFrameworkCore.Audit;

/// <summary>
///     Represents an extension for configuring audit options in Entity Framework Core.
/// </summary>
public sealed class AuditOptionsExtension : IDbContextOptionsExtension
{
    /// <summary>
    ///     Represents a private field used to store extension-specific information
    ///     about the <see cref="AuditOptionsExtension" /> used within Entity Framework Core.
    /// </summary>
    /// <remarks>
    ///     This field holds an instance of <see cref="DbContextOptionsExtensionInfo" />
    ///     and defines metadata for the AuditOptionsExtension.
    /// </remarks>
    private DbContextOptionsExtensionInfo? _info;

    /// <summary>
    ///     Represents an extension for enabling audit functionality in EF Core DbContext.
    /// </summary>
    public AuditOptionsExtension(bool useAuditEntry, string jsonColumnType)
    {
        ArgumentVerifier.NotNull(jsonColumnType, nameof(jsonColumnType));

        UseAuditEntry = useAuditEntry;
        JsonColumnType = jsonColumnType;
    }

    /// <summary>
    ///     Gets a value indicating whether the audit entry functionality is enabled.
    /// </summary>
    /// <value>
    ///     A boolean value where <c>true</c> indicates the audit entry functionality is enabled,
    ///     and <c>false</c> indicates it is disabled.
    /// </value>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the property is accessed before being correctly initialized.
    /// </exception>
    public bool UseAuditEntry { get; }

    /// <summary>
    ///     Specifies the type of the JSON column used for audit entries.
    /// </summary>
    /// <remarks>
    ///     This property defines the database-specific column type for storing JSON data in audit entries.
    ///     It is used to ensure proper mapping and compatibility with the underlying database when saving
    ///     JSON values for fields such as OldValues, NewValues, and Metadata.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the value assigned is null or empty.
    /// </exception>
    /// <value>
    ///     Returns the database type string used to map JSON columns.
    /// </value>
    public string JsonColumnType { get; }

    /// <summary>
    ///     Gets the <see cref="DbContextOptionsExtensionInfo" /> associated with this extension.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="DbContextOptionsExtensionInfo" /> representing detailed metadata and behavior
    ///     for this database context options extension.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if required arguments for creating the extension info are not provided.
    /// </exception>
    public DbContextOptionsExtensionInfo Info => _info ??= new AuditOptionsExtensionInfo(this);


    /// <summary>
    ///     Configures the specified service collection to include services required for auditing.
    ///     This method is called to register any necessary services for the audit functionality in the dependency injection
    ///     container.
    /// </summary>
    /// <param name="services">The service collection to which the audit-related services will be added.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> parameter is null.</exception>
    public void ApplyServices(IServiceCollection services)
    {
    }

    /// <summary>
    ///     Validates the provided database context options to ensure they meet the requirements
    ///     of the current audit options settings.
    /// </summary>
    /// <param name="options">The database context options to be validated.</param>
    /// <exception cref="ArgumentNullException">Thrown if the required options are null or invalid.</exception>
    public void Validate(IDbContextOptions options)
    {
    }

    /// <summary>
    ///     Represents an extension providing auditing options for Entity Framework Core.
    /// </summary>
    private sealed class AuditOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        /// <summary>
        ///     Represents an instance of the audit options extension used to store additional configuration
        ///     or metadata specific to auditing functionality in the Entity Framework context.
        /// </summary>
        private readonly AuditOptionsExtension _optionsExtension;

        /// <summary>
        ///     Represents an Entity Framework Core database context options extension
        ///     for configuring auditing features.
        /// </summary>
        public AuditOptionsExtensionInfo(AuditOptionsExtension optionsExtension) : base(optionsExtension)
        {
            ArgumentVerifier.NotNull(optionsExtension, nameof(optionsExtension));

            _optionsExtension = optionsExtension;
        }

        /// <summary>
        ///     Indicates whether the extension is a database provider.
        /// </summary>
        /// <returns>
        ///     A boolean value indicating whether the extension is a database provider.
        ///     Always returns <c>false</c> as this extension is not a database provider.
        /// </returns>
        public override bool IsDatabaseProvider => false;

        /// <summary>
        ///     Represents the log fragment information for the <see cref="AuditOptionsExtension" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when required properties of the related <see cref="AuditOptionsExtension" /> are not properly initialized.
        /// </exception>
        /// <returns>
        ///     A string representing the logging details, typically including values for auditing options such as
        ///     <c>UseAuditEntry</c> and <c>JsonColumnType</c>.
        /// </returns>
        public override string LogFragment =>
            $"UseAuditEntry={_optionsExtension.UseAuditEntry}, JsonColumnType={_optionsExtension.JsonColumnType}";

        /// <summary>
        ///     Computes the hash code for the service provider configuration based on the audit options.
        /// </summary>
        /// <returns>
        ///     An integer representing the computed hash code from the audit options.
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        ///     Thrown if the audit options are not properly initialized or contain null values.
        /// </exception>
        public override int GetServiceProviderHashCode() =>
            HashCode.Combine(_optionsExtension.UseAuditEntry, _optionsExtension.JsonColumnType);

        /// <summary>
        ///     Determines whether the same service provider can be used for the current and specified
        ///     <see cref="DbContextOptionsExtensionInfo" /> instances based on their configuration.
        /// </summary>
        /// <param name="other">
        ///     The other <see cref="DbContextOptionsExtensionInfo" /> instance to compare with the current instance.
        /// </param>
        /// <returns>
        ///     True if the same service provider can be used; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="other" /> is null.
        /// </exception>
        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) =>
            other is AuditOptionsExtensionInfo auditOptionsExtensionInfo &&
            _optionsExtension.UseAuditEntry == auditOptionsExtensionInfo._optionsExtension.UseAuditEntry &&
            _optionsExtension.JsonColumnType == auditOptionsExtensionInfo._optionsExtension.JsonColumnType;

        /// <summary>
        ///     Populates the debug information dictionary with details specific to the AuditOptionsExtension instance.
        /// </summary>
        /// <param name="debugInfo">
        ///     A dictionary where debug information will be added. Keys represent debug property names, and
        ///     values represent their corresponding data.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="debugInfo" /> is null.</exception>
        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["Audit:UseAuditEntry"] = _optionsExtension.UseAuditEntry.ToString();
            debugInfo["Audit:JsonColumnType"] = _optionsExtension.JsonColumnType;
        }
    }
}