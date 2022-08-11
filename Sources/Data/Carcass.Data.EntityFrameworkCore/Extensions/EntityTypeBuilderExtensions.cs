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

using Carcass.Core;
using Carcass.Data.EntityFrameworkCore.Entities.Abstracts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carcass.Data.EntityFrameworkCore.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static void ConfigureIdentifiableEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, IIdentifiableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.HasKey(e => e.Id);
    }

    public static void ConfigureAuditableEntity<TAuditableEntity>(this EntityTypeBuilder<TAuditableEntity> builder)
        where TAuditableEntity : class, IAuditableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.Property(ae => ae.CreatedBy);
        builder.Property(ae => ae.CreatedAt).IsRequired();
        builder.Property(ae => ae.UpdatedBy);
        builder.Property(ae => ae.UpdatedAt);
    }

    public static void ConfigureSoftDeletableEntity<TSoftDeletableEntity>(this EntityTypeBuilder<TSoftDeletableEntity> builder)
        where TSoftDeletableEntity : class, ISoftDeletableEntity
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        builder.Property(sde => sde.IsDeleted).IsRequired();
    }
}