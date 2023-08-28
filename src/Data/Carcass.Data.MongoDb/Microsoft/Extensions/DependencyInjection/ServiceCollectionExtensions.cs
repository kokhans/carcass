// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassMongoDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<MongoDbOptions, MongoClient>? mongoClientFactory = default,
        Func<MongoDbOptions, MongoClient, IMongoDatabase>? mongoDatabaseFactory = default,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<MongoDbOptions>(configuration.GetSection("Carcass:MongoDb"));

        BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(
            typeof(decimal?),
            new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128))
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