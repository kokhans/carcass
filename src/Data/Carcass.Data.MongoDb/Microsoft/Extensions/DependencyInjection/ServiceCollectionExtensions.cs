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
using Carcass.Data.Core.EventSourcing.Checkpoints.Repositories.Abstracts;
using Carcass.Data.Core.EventSourcing.Snapshotting.Repositories.Abstracts;
using Carcass.Data.MongoDb.Checkpoints.Repositories;
using Carcass.Data.MongoDb.Options;
using Carcass.Data.MongoDb.Sessions;
using Carcass.Data.MongoDb.Sessions.Abstracts;
using Carcass.Data.MongoDb.Snapshotting.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for registering MongoDB-related services into an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds MongoDB integration to the IServiceCollection by configuring and registering necessary MongoDB services,
    ///     including MongoClient and IMongoDatabase.
    /// </summary>
    /// <param name="services">
    ///     The IServiceCollection where the MongoDB-related services will be registered.
    /// </param>
    /// <param name="configuration">
    ///     The IConfiguration instance used to retrieve MongoDB configuration settings.
    /// </param>
    /// <param name="mongoClientFactory">
    ///     An optional factory method to create a custom MongoClient instance. If not provided, a default MongoClient
    ///     is created based on the configuration.
    /// </param>
    /// <param name="mongoDatabaseFactory">
    ///     An optional factory method to create a custom IMongoDatabase instance. If not provided, a default
    ///     MongoDatabase is created based on the configuration.
    /// </param>
    /// <param name="lifetime">
    ///     Specifies the ServiceLifetime for the registered services. Defaults to Singleton.
    /// </param>
    /// <returns>
    ///     The modified IServiceCollection instance including the MongoDB services.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the provided services or configuration parameters are null.
    /// </exception>
    public static IServiceCollection AddCarcassMongoDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<MongoDbOptions, MongoClient>? mongoClientFactory = null,
        Func<MongoDbOptions, MongoClient, IMongoDatabase>? mongoDatabaseFactory = null,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<MongoDbOptions>(configuration.GetSection("Carcass:MongoDb"));

        BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128))
        );

        // Register MongoClient.
        if (mongoClientFactory is null)
            services.Add(ServiceDescriptor.Describe(
                    typeof(MongoClient), sp =>
                    {
                        IOptions<MongoDbOptions> options =
                            sp.GetRequiredService<IOptions<MongoDbOptions>>();

                        return new MongoClient(options.Value.ConnectionString);
                    },
                    lifetime
                )
            );
        else
            services.Add(ServiceDescriptor.Describe(
                    typeof(MongoClient), sp =>
                    {
                        IOptions<MongoDbOptions> options =
                            sp.GetRequiredService<IOptions<MongoDbOptions>>();

                        return mongoClientFactory(options.Value);
                    },
                    lifetime
                )
            );

        // Register IMongoDatabase.
        if (mongoDatabaseFactory is null)
            services.Add(ServiceDescriptor.Describe(
                    typeof(IMongoDatabase), sp =>
                    {
                        IOptions<MongoDbOptions> options =
                            sp.GetRequiredService<IOptions<MongoDbOptions>>();

                        MongoClient mongoClient = sp.GetRequiredService<MongoClient>();

                        return mongoClient.GetDatabase(options.Value.DatabaseName);
                    },
                    lifetime
                )
            );
        else
            services.Add(ServiceDescriptor.Describe(
                    typeof(IMongoDatabase), sp =>
                    {
                        IOptions<MongoDbOptions> options =
                            sp.GetRequiredService<IOptions<MongoDbOptions>>();

                        MongoClient mongoClient = sp.GetRequiredService<MongoClient>();

                        return mongoDatabaseFactory(options.Value, mongoClient);
                    },
                    lifetime
                )
            );

        return services;
    }

    /// <summary>
    ///     Adds the MongoDB session service to the dependency injection container.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add the service to.
    /// </param>
    /// <param name="lifetime">
    ///     The lifetime of the service. Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>
    ///     The updated <see cref="IServiceCollection" /> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> argument is null.
    /// </exception>
    public static IServiceCollection AddCarcassMongoDbSession(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(IMongoDbSession),
                typeof(MongoDbSession),
                lifetime
            )
        );

        return services;
    }

    /// <summary>
    ///     Registers the MongoDB-based checkpoint repository implementation with the specified service collection.
    /// </summary>
    /// <param name="services">The service collection where the checkpoint repository will be registered.</param>
    /// <param name="lifetime">The lifetime of the registered service. Defaults to singleton.</param>
    /// <returns>The modified service collection with the checkpoint repository service registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the service collection parameter is null.</exception>
    public static IServiceCollection AddCarcassMongoDbCheckpointRepository(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(ICheckpointRepository),
                typeof(MongoDbCheckpointRepository),
                lifetime
            )
        );

        return services;
    }

    /// <summary>
    ///     Adds the MongoDB snapshot repository implementation to the dependency injection container.
    /// </summary>
    /// <param name="services">The dependency injection service collection.</param>
    /// <param name="lifetime">
    ///     The desired lifetime of the snapshot repository within the service container. Default is
    ///     Singleton.
    /// </param>
    /// <returns>The modified service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassMongoDbSnapshotRepository(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(ISnapshotRepository),
                typeof(MongoDbSnapshotRepository),
                lifetime
            )
        );

        return services;
    }
}