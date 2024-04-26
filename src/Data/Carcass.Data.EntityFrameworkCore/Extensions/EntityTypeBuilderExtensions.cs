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
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable UnusedMember.Global

namespace Carcass.Data.EntityFrameworkCore.Extensions;

/// <summary>
///     Provides extension methods for configuring different types of entities
///     using the <see cref="Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder{TEntity}" />.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    ///     Configures a given entity type as an identifiable entity by setting its primary key to the "Id" property.
    /// </summary>
    /// <typeparam name="TIdentifiableEntity">
    ///     The type of the entity to configure, which must implement <see cref="IIdentifiableEntity" />.
    /// </typeparam>
    /// <param name="builder">
    ///     An <see cref="EntityTypeBuilder{TEntity}" /> used to configure the entity type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="builder" /> is null.
    /// </exception>
    public static void ConfigureIdentifiableEntity<TIdentifiableEntity>(
        this EntityTypeBuilder<TIdentifiableEntity> builder)
        where TIdentifiableEntity : class, IIdentifiableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.HasKey(e => e.Id);
    }

    /// <summary>
    ///     Configures the entity as a tenantifiable entity by setting up the tenant-related properties.
    ///     This method applies metadata constraints and configurations for an entity that implements
    ///     <see cref="ITenantifiableEntity" />.
    /// </summary>
    /// <typeparam name="TTenantifiableEntity">
    ///     The type of the tenantifiable entity being configured. This type must implement <see cref="ITenantifiableEntity" />
    ///     .
    /// </typeparam>
    /// <param name="builder">
    ///     The <see cref="EntityTypeBuilder{TTenantifiableEntity}" /> used to configure the entity model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="builder" /> argument is null.
    /// </exception>
    public static void ConfigureTenantifiableEntity<TTenantifiableEntity>(
        this EntityTypeBuilder<TTenantifiableEntity> builder)
        where TTenantifiableEntity : class, ITenantifiableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.Property(e => e.TenantId).IsRequired(false);
    }

    /// <summary>
    ///     Configures the properties for an auditable entity to ensure proper database mapping
    ///     and enforce constraints for audit-related fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt).
    /// </summary>
    /// <typeparam name="TAuditableEntity">
    ///     The type of the entity implementing <see cref="IAuditableEntity" />.
    /// </typeparam>
    /// <param name="builder">
    ///     The <see cref="EntityTypeBuilder{T}" /> instance used to configure the entity type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="builder" /> is null.
    /// </exception>
    public static void ConfigureAuditableEntity<TAuditableEntity>(this EntityTypeBuilder<TAuditableEntity> builder)
        where TAuditableEntity : class, IAuditableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.Property(ae => ae.CreatedBy).IsRequired(false);
        builder.Property(ae => ae.CreatedAt).IsRequired();
        builder.Property(ae => ae.UpdatedBy).IsRequired(false);
        builder.Property(ae => ae.UpdatedAt).IsRequired(false);
    }

    /// <summary>
    ///     Configures an entity type to include properties and behaviors related to soft deletion.
    ///     Sets up the required properties for managing soft deletable entities, particularly the "IsDeleted" flag.
    /// </summary>
    /// <typeparam name="TSoftDeletableEntity">
    ///     The type of the entity that implements <see cref="ISoftDeletableEntity" />.
    /// </typeparam>
    /// <param name="builder">
    ///     The builder used to configure the entity type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="builder" /> parameter is null.
    /// </exception>
    public static void ConfigureSoftDeletableEntity<TSoftDeletableEntity>(
        this EntityTypeBuilder<TSoftDeletableEntity> builder)
        where TSoftDeletableEntity : class, ISoftDeletableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.Property(sde => sde.IsDeleted).IsRequired();
    }
}