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
using Carcass.Data.Core.Checkpoints.Repositories.Abstracts;
using Carcass.Data.Core.Snapshotting.Repositories.Abstracts;
using Carcass.Data.MongoDb.Checkpoints.Repositories;
using Carcass.Data.MongoDb.Conductors;
using Carcass.Data.MongoDb.Conductors.Abstracts;
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
    public static IServiceCollection AddCarcassMongoDbConductor(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<MongoDbOptions, Tuple<MongoClient, IMongoDatabase>>? factory = default,
        bool reloadOptions = false
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Configure<MongoDbOptions>(configuration.GetSection("Carcass:MongoDb"));

        BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(
            typeof(decimal?),
            new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128))
        );

        if (reloadOptions)
            services.AddSingleton<IMongoDbConductor>(sp => new MongoDbConductor(
                    sp.GetRequiredService<IOptionsMonitor<MongoDbOptions>>(),
                    factory
                )
            );
        else
            services.AddSingleton<IMongoDbConductor>(sp => new MongoDbConductor(
                    sp.GetRequiredService<IOptions<MongoDbOptions>>(),
                    factory
                )
            );

        return services.AddSingleton<IMongoDbSession, MongoDbSession>();
    }

    public static IServiceCollection AddCarcassMongoDbSession(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IMongoDbSession, MongoDbSession>();
    }

    public static IServiceCollection AddCarcassMongoDbCheckpointRepository(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ICheckpointRepository, MongoDbCheckpointRepository>();
    }

    public static IServiceCollection AddCarcassMongoDbSnapshotRepository(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ISnapshotRepository, MongoDbSnapshotRepository>();
    }
}