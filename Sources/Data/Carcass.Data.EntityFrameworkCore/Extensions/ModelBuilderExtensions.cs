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

using System.Linq.Expressions;
using System.Reflection;
using Carcass.Core;
using Carcass.Core.Extensions;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Carcass.Data.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
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

    public static ModelBuilder ApplyIsDeletedQueryFilter(this ModelBuilder modelBuilder)
    {
        ArgumentVerifier.NotNull(modelBuilder, nameof(modelBuilder));

        foreach (IMutableEntityType mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            IMutableProperty? mutableProperty = mutableEntityType.FindProperty(nameof(ISoftDeletableEntity.IsDeleted));
            if (mutableProperty is not null &&
                mutableProperty.ClrType == typeof(bool) &&
                mutableProperty.PropertyInfo is not null
               )
            {
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
        }

        return modelBuilder;
    }

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
        if (applyConfigurationMethodInfo is not null)
            foreach (Type type in assembly.GetTypes().Where(
                         t1 => t1.IsClass && !t1.IsAbstract && !t1.ContainsGenericParameters
                     )
                    )
            foreach (Type @interface in type.GetInterfaces().Where(
                         t2 => t2.IsGenericType && t2.GetGenericTypeDefinition() == entityTypeConfigurationType
                     )
                    )
            {
                MethodInfo applyConcreteMethod = applyConfigurationMethodInfo
                    .MakeGenericMethod(@interface.GenericTypeArguments.First());
                applyConcreteMethod.Invoke(modelBuilder, new[] {Activator.CreateInstance(type)});
            }

        return modelBuilder;
    }
}