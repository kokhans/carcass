# Carcass [![GitHub](https://img.shields.io/github/license/kokhans/carcass?style=flat-square)](LICENSE)

Carcass is a free, open-source, community-focused infrastructure framework based on .NET 7 for building modern applications.

## Status

### Alpha

The software is complete enough for internal testing. This is typically done by people other than the software engineers who wrote it, but still within the same organization or community that developed the software.

## Features

Carcass is a feature-rich infrastructure framework. It provides a set of modules that enable the development of cloud, web, console, desktop, and mobile applications.

- Modular
- Cross-Cutting Concerns
- Domain-Driven Design
- CQRS
- Event Sourcing
- Multitenancy
- Distributed Cache
- Microservices
- Audit Logging

## Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

## Packages

- [Carcass.Core](https://www.nuget.org/packages/Carcass.Core) - Core abstactions, interfaces and types used by Carcass.* libraries.

- [Carcass.Metadata](https://www.nuget.org/packages/Carcass.Core) - Metadata toolchain.

- [Carcass.SignalR](https://www.nuget.org/packages/Carcass.SignalR) - SignalR toolchain.

- [Carcass.Swashbuckle](https://www.nuget.org/packages/Carcass.Swashbuckle) - Swashbuckle toolchain.

- [Carcass.Data.Core](https://www.nuget.org/packages/Carcass.Data.Core) - Domain-Driven Design, CQRS and Event Sourcing core abstactions, interfaces and types used by Carcass.Data.* libraries.

- [Carcass.Data.Elasticsearch](https://www.nuget.org/packages/Carcass.Data.Elasticsearch) - Domain-Driven Design, CQRS and Event Sourcing implementation based on Elasticsearch.

- [Carcass.Data.EntityFrameworkCore](https://www.nuget.org/packages/Carcass.Data.EntityFrameworkCore) - Domain-Driven Design, CQRS and Event Sourcing implementation based on EntityFramework Core.

- [Carcass.Data.EventStoreDb](https://www.nuget.org/packages/Carcass.Data.EventStoreDb) - Domain-Driven Design, CQRS and Event Sourcing implementation based on EventStoreDB.

- [Carcass.Data.MongoDb](https://www.nuget.org/packages/Carcass.Data.MongoDb) - Domain-Driven Design, CQRS and Event Sourcing implementation based on MongoDB.

- [Carcass.DistributedCache.Core](https://www.nuget.org/packages/Carcass.DistributedCache.Core) - Distributed cache core abstactions, interfaces and types used by Carcass.DistributedCache.* libraries.

- [Carcass.DistributedCache.Redis](https://www.nuget.org/packages/Carcass.DistributedCache.Redis) - Redis distributed cache provider implementation based on Microsoft.Extensions.Caching.Redis.

- [Carcass.Json.Core](https://www.nuget.org/packages/Carcass.Json.Core) - JSON core abstactions, interfaces and types used by Carcass.Json.* libraries.

- [Carcass.Json.NewtonsoftJson](https://www.nuget.org/packages/Carcass.Json.NewtonsoftJson) - JSON provider implementation based on Newtonsoft.Json.

- [Carcass.Json.SystemTextJson](https://www.nuget.org/packages/Carcass.Json.SystemTextJson) - JSON provider implementation based on System.Text.Json.

- [Carcass.Logging.Core](https://www.nuget.org/packages/Carcass.Logging.Core) - Logging core abstactions, interfaces and types used by Carcass.Logging.* libraries.

- [Carcass.Mapping.Core](https://www.nuget.org/packages/Carcass.Mapping.Core) - Mapping core abstactions, interfaces and types used by Carcass.Mapping.* libraries.

- [Carcass.Mapping.AutoMapper](https://www.nuget.org/packages/Carcass.Mapping.AutoMapper) - Mapper provider implementation based on AutoMapper.

- [Carcass.Multitenancy.Core](https://www.nuget.org/packages/Carcass.Multitenancy.Core) - Multitenancy core abstactions, interfaces and types used by Carcass.Multitenancy.* libraries.

- [Carcass.Mvc.Core](https://www.nuget.org/packages/Carcass.Mvc.Core) - MVC core abstactions, interfaces and types used by Carcass.Mvc.* libraries.

- [Carcass.Yaml.Core](https://www.nuget.org/packages/Carcass.Yaml.Core) - YAML core abstactions, interfaces and types used by Carcass.Yaml.* libraries.

- [Carcass.Yaml.DotNetYaml](https://www.nuget.org/packages/Carcass.Yaml.DotNetYaml) - YAML provider implementation based on YamlDotNet.

## License

This project is licensed under the [MIT license](LICENSE).