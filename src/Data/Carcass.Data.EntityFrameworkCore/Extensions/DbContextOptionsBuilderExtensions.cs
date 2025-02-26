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
using Carcass.Core.Extensions;
using Carcass.Data.EntityFrameworkCore.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Carcass.Data.EntityFrameworkCore.Extensions;

/// <summary>
///     Provides extension methods for configuring DbContext options with additional features specific to
///     Carcass.Data.EntityFrameworkCore.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    ///     Configures the DbContext to use Carcass audit features for tracking entity changes.
    /// </summary>
    /// <param name="optionsBuilder">The DbContextOptionsBuilder to configure.</param>
    /// <param name="useAuditEntry">
    ///     Indicates whether auditing should be enabled. Defaults to true.
    /// </param>
    /// <param name="jsonColumnType">
    ///     Specifies the JSON column type to use for storing audit data. Defaults to Nvarchar.
    /// </param>
    /// <returns>The configured DbContextOptionsBuilder instance.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="optionsBuilder" /> is null.
    /// </exception>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static DbContextOptionsBuilder UseCarcassAudit(
        this DbContextOptionsBuilder optionsBuilder,
        bool useAuditEntry = true,
        JsonColumnType jsonColumnType = JsonColumnType.Nvarchar
    )
    {
        ArgumentVerifier.NotNull(optionsBuilder, nameof(optionsBuilder));

        AuditOptionsExtension extension = optionsBuilder.Options.FindExtension<AuditOptionsExtension>()
                                          ?? new AuditOptionsExtension(useAuditEntry, jsonColumnType.GetDescription()!);

        ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    /// <summary>
    ///     Retrieves the <see cref="AuditOptionsExtension" /> instance from the specified
    ///     <see cref="DbContextOptions" />, if it has been configured.
    /// </summary>
    /// <param name="options">
    ///     The <see cref="DbContextOptions" /> from which to find the <see cref="AuditOptionsExtension" />.
    /// </param>
    /// <returns>
    ///     The <see cref="AuditOptionsExtension" /> instance found in the <paramref name="options" />,
    ///     or null if no such extension is configured.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="options" /> argument is null.
    /// </exception>
    public static AuditOptionsExtension? FindAuditOptionsExtension(this DbContextOptions options)
    {
        ArgumentVerifier.NotNull(options, nameof(options));

        return options.FindExtension<AuditOptionsExtension>();
    }
}