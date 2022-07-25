# Carcass [![GitHub](https://img.shields.io/github/license/kokhans/carcass?style=flat-square)](LICENSE)

`Carcass` is a free, open-source, community-focused infrastructure framework based on `.NET 6` for building modern applications.

## Status

### Pre-Alpha

The software is still under active development and not feature complete or ready for consumption by anyone other than software developers. There may be milestones during the pre-alpha which deliver specific sets of functionality, and nightly builds for other developers or users who are comfortable living on the absolute bleeding edge.

## Features

`Carcass` is a feature-rich infrastructure framework. It provides a set of modules that enable the development of cloud, web, console, desktop, and mobile applications.

- Modular
- Cross-Cutting Concerns
- Domain-Driven Design
- CQRS
- Event Sourcing
- Multitenancy
- Distributed Cache
- Microservices
- Audit Logging
- Object Storage

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

## Packages

### Core

Core abstactions, interfaces and types used by `Carcass.*` libraries.

### LocalStorage

Local storage toolchain.

### Metadata

Metadata toolchain.

### Swashbuckle

`Swashbuckle` toolchain.

### Cli.Logging.Core

CLI logging core abstactions, interfaces and types used by `Carcass.Cli.Logging.*` libraries.

### Cli.Logging.Spectre

CLI logger implementation based on `Spectre.Console`.

### Data.Core

Domain-Driven Design, CQRS and Event Sourcing core abstactions, interfaces and types used by `Carcass.Data.*` libraries.

### Data.Elasticsearch

Domain-Driven Design, CQRS and Event Sourcing implementation based on `Elasticsearch`.

### Data.EntityFrameworkCore

Domain-Driven Design, CQRS and Event Sourcing implementation based on `EntityFramework Core`.

### Data.EventStoreDb

Domain-Driven Design, CQRS and Event Sourcing implementation based on `EventStoreDB`.

### Data.MongoDb

Domain-Driven Design, CQRS and Event Sourcing implementation based on `MongoDB`.

### DistributedCache.Core

Distributed cache core abstactions, interfaces and types used by `Carcass.DistributedCache.*` libraries.

### DistributedCache.Redis

`Redis` distributed cache provider implementation based on `Microsoft.Extensions.Caching.Redis`.

### FrontMatter.Core

Front matter core abstactions, interfaces and types used by `Carcass.FrontMatter.*` libraries.

### FrontMatter.Markdown

`Markdown` front matter parser implementation based on `Markdig`.

### FrontMatter.Razor

`Razor` front matter parser implementation.

### Json.Core

`JSON` core abstactions, interfaces and types used by `Carcass.Json.*` libraries.

### Json.NewtonsoftJson

`JSON` provider implementation based on `Newtonsoft.Json`.

### Json.SystemTextJson

`JSON` provider implementation based on `System.Text.Json`.

### Logging.Core

Logging core abstactions, interfaces and types used by `Carcass.Logging.*` libraries.

### Mapping.Core

Mapping core abstactions, interfaces and types used by `Carcass.Mapping.*` libraries.

### Mapping.AutoMapper

Mapper provider implementation based on `AutoMapper`.

### Multitenancy.Core

Multitenancy core abstactions, interfaces and types used by `Carcass.Multitenancy.*` libraries.

### Mvc.Core

`MVC` core abstactions, interfaces and types used by `Carcass.Mvc.*` libraries.

### Mvc.Razor.Rendering

`Razor` view renderer.

### ObjectStorage.Core

Object storage core abstactions, interfaces and types used by `Carcass.ObjectStorage.*` libraries.

### ObjectStorage.Minio

Object storage provider implementation based on `Minio`.

### Yaml.Core

`YAML` core abstactions, interfaces and types used by `Carcass.Yaml.*` libraries.

### Yaml.DotNetYaml

`YAML` provider implementation based on `YamlDotNet`.

## License

This project is licensed under the [MIT license](LICENSE).