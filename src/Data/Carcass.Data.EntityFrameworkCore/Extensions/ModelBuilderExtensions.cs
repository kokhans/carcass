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

using System.Linq.Expressions;
using System.Reflection;
using Carcass.Core;
using Carcass.Core.Extensions;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace Carcass.Data.EntityFrameworkCore.Extensions;

/// <summary>
///     Provides extension methods for <see cref="ModelBuilder" /> to assist with advanced model configuration in Entity
///     Framework Core.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies a snake_case naming convention to all tables, columns, keys, foreign keys, and indexes
    ///     within the specified <see cref="ModelBuilder" /> instance.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder" /> instance to apply the snake_case convention to.</param>
    /// <returns>The <see cref="ModelBuilder" /> instance after applying the snake_case naming convention.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modelBuilder" /> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an entity type is not mapped to a table, resulting in a null or empty table name.
    /// </exception>
    public static ModelBuilder ApplySnakeCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));

        foreach (IMutableEntityType mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            string? tableName = mutableEntityType.GetTableName()?.ToSnakeCase();
            string? schema = mutableEntityType.GetSchema();

            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException(
                    $"Entity type {mutableEntityType.Name} is not mapped to the table.");

            mutableEntityType.SetTableName(tableName);

            foreach (IMutableProperty mutableProperty in mutableEntityType.GetProperties())
                mutableProperty.SetColumnName(
                    mutableProperty.GetColumnName(StoreObjectIdentifier.Table(tableName, schema)).ToSnakeCase()
                );

            foreach (IMutableKey mutableKey in mutableEntityType.GetKeys())
                mutableKey.SetName(mutableKey.GetName().ToSnakeCase());

            foreach (IMutableForeignKey mutableForeignKey in mutableEntityType.GetForeignKeys())
                mutableForeignKey.SetConstraintName(mutableForeignKey.GetConstraintName().ToSnakeCase());

            foreach (IMutableIndex mutableIndex in mutableEntityType.GetIndexes())
                mutableIndex.SetDatabaseName(mutableIndex.GetDatabaseName().ToSnakeCase());
        }

        return modelBuilder;
    }

    /// <summary>
    ///     Removes the "AspNet" prefix from table names in the model's entity types.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder" /> used to configure the entity types.</param>
    /// <returns>The updated <see cref="ModelBuilder" /> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modelBuilder" /> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if any entity type does not have a valid table name assigned.
    /// </exception>
    public static ModelBuilder DeleteAspNetPrefix(this ModelBuilder modelBuilder)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));

        const string tableNamePrefix = "AspNet";

        IList<IMutableEntityType> mutableEntityTypes = modelBuilder.Model.GetEntityTypes().ToList();
        foreach (IMutableEntityType mutableEntityType in mutableEntityTypes)
        {
            string? tableName = mutableEntityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException(
                    $"Entity type {mutableEntityType.Name} is not mapped to the table.");

            string? newTableName = tableName.StartsWith(tableNamePrefix)
                ? tableName[tableNamePrefix.Length..]
                : mutableEntityType.GetTableName();
            mutableEntityType.SetTableName(newTableName);
        }

        return modelBuilder;
    }

    /// <summary>
    ///     Applies a query filter to all entities implementing the ISoftDeletableEntity interface,
    ///     ensuring that only entities with IsDeleted set to false are included in query results.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The <see cref="ModelBuilder" /> used to apply the query filter to the entity types.
    /// </param>
    /// <returns>
    ///     The modified <see cref="ModelBuilder" /> for further configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="modelBuilder" /> is null.
    /// </exception>
    public static ModelBuilder ApplyIsDeletedQueryFilter(this ModelBuilder modelBuilder)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));

        foreach (IMutableEntityType mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            IMutableProperty? mutableProperty = mutableEntityType.FindProperty(nameof(ISoftDeletableEntity.IsDeleted));
            if (mutableProperty is null ||
                mutableProperty.ClrType != typeof(bool) ||
                mutableProperty.PropertyInfo is null) continue;
            ParameterExpression parameterExpression = Expression.Parameter(
                mutableEntityType.ClrType,
                mutableEntityType.Name.ToLower()
            );
            LambdaExpression lambdaExpression = Expression.Lambda(
                Expression.Equal(
                    Expression.Property(parameterExpression, mutableProperty.PropertyInfo),
                    Expression.Constant(false)), parameterExpression
            );
            mutableEntityType.SetQueryFilter(lambdaExpression);
        }

        return modelBuilder;
    }

    /// <summary>
    ///     Applies entity type configurations from the specified assembly to the <see cref="ModelBuilder" /> instance.
    ///     This method scans for all implementation types of <see cref="IEntityTypeConfiguration{TEntity}" /> within the
    ///     provided assembly
    ///     and applies them to the model builder.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The <see cref="ModelBuilder" /> instance used to apply the configurations.
    /// </param>
    /// <param name="assembly">
    ///     The assembly to scan for <see cref="IEntityTypeConfiguration{TEntity}" /> implementations.
    /// </param>
    /// <returns>
    ///     The updated <see cref="ModelBuilder" /> instance with the configurations applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="modelBuilder" /> or <paramref name="assembly" /> is null.
    /// </exception>
    public static ModelBuilder ApplyEntityConfigurations(this ModelBuilder modelBuilder, Assembly assembly)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));
        ArgumentVerifier.NotNull(assembly, nameof(assembly));

        Type entityTypeConfigurationType = typeof(IEntityTypeConfiguration<>);
        MethodInfo? applyConfigurationMethodInfo = modelBuilder.GetType().GetMethods().FirstOrDefault(mi =>
            mi.Name.Equals("ApplyConfiguration", StringComparison.InvariantCultureIgnoreCase) &&
            mi.GetParameters().Any(pi =>
                pi.ParameterType.IsGenericType &&
                pi.ParameterType.GetGenericTypeDefinition() == entityTypeConfigurationType
            )
        );
        if (applyConfigurationMethodInfo is null)
            return modelBuilder;

        foreach (Type type in assembly.GetTypes().Where(
                     t1 => t1 is {IsClass: true, IsAbstract: false, ContainsGenericParameters: false}
                 )
                )
        foreach (Type @interface in type.GetInterfaces().Where(
                     t2 => t2.IsGenericType && t2.GetGenericTypeDefinition() == entityTypeConfigurationType
                 )
                )
        {
            MethodInfo applyConcreteMethod = applyConfigurationMethodInfo
                .MakeGenericMethod(@interface.GenericTypeArguments.First());
            applyConcreteMethod.Invoke(modelBuilder, [Activator.CreateInstance(type)]);
        }

        return modelBuilder;
    }
}