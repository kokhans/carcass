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
using Carcass.Data.Core.Audit;
using Carcass.Data.EntityFrameworkCore.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Carcass.Data.EntityFrameworkCore.Audit;

/// <summary>
///     Configures the AuditEntry entity for use with Entity Framework Core.
/// </summary>
/// <remarks>
///     This class defines how the AuditEntry entity is mapped to the database and includes
///     configurations for properties, value converters, and required fields.
/// </remarks>
public sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    /// <summary>
    ///     Represents the private field that provides configuration options
    ///     related to auditing behavior for the AuditEntry entity.
    /// </summary>
    /// <remarks>
    ///     The field is used to store an instance of <see cref="AuditOptionsExtension" />
    ///     which contains specific settings, such as JSON column types, utilized
    ///     in the configuration of database entities related to auditing.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown during instantiation of <see cref="AuditEntryConfiguration" />
    ///     if the injected instance of <see cref="AuditOptionsExtension" /> is null.
    /// </exception>
    private readonly AuditOptionsExtension _auditOptionsExtension;

    /// <summary>
    ///     Provides JSON serialization and deserialization functionality
    ///     for converting data to and from JSON format within the context
    ///     of auditing entities.
    /// </summary>
    private readonly IJsonProvider _jsonProvider;

    /// <summary>
    ///     Configures the entity type <see cref="AuditEntry" /> for use with Entity Framework Core.
    /// </summary>
    public AuditEntryConfiguration(
        IJsonProvider jsonProvider,
        AuditOptionsExtension auditOptionsExtension
    )
    {
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));
        ArgumentVerifier.NotNull(auditOptionsExtension, nameof(auditOptionsExtension));

        _jsonProvider = jsonProvider;
        _auditOptionsExtension = auditOptionsExtension;
    }

    /// <summary>
    ///     Configures the entity type for <see cref="AuditEntry" /> in the database context.
    /// </summary>
    /// <param name="builder">
    ///     The <see cref="EntityTypeBuilder{TEntity}" /> used to configure the <see cref="AuditEntry" /> entity.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="builder" /> argument is null.
    /// </exception>
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.ConfigureIdentifiableEntity();

        builder.Property(ae => ae.EntityName).IsRequired(false);
        builder.Property(ae => ae.PrimaryKey).IsRequired(false);
        builder.Property(ae => ae.Timestamp).IsRequired();

        ValueConverter<Dictionary<string, object?>, string> valueConverter = new(
            d => _jsonProvider.Serialize(d),
            s => _jsonProvider.Deserialize<Dictionary<string, object?>>(s)
        );

        builder.Property(ae => ae.OldValues)
            .HasConversion(valueConverter)
            .HasColumnType(_auditOptionsExtension.JsonColumnType)
            .IsRequired();

        builder.Property(ae => ae.NewValues)
            .HasConversion(valueConverter)
            .HasColumnType(_auditOptionsExtension.JsonColumnType)
            .IsRequired();

        builder.Property(ae => ae.Metadata)
            .HasConversion(valueConverter)
            .HasColumnType(_auditOptionsExtension.JsonColumnType)
            .IsRequired();

        builder.Property(ae => ae.OperationType)
            .HasConversion(new EnumToStringConverter<OperationType>())
            .IsRequired();
    }
}