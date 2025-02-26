# Carcass [![GitHub](https://img.shields.io/github/license/kokhans/carcass?style=flat-square)](LICENSE)

**Carcass** is a **free, open-source, community-driven infrastructure framework** built on **.NET 9** for creating **modern, scalable applications**.

<img src="./docs/images/carcass-logo.png" alt="Carcass Logo" />

## ✨ Features

Carcass offers a wide range of features designed to **accelerate application development** and **ensure maintainability**.

### 🧩 Modular Architecture

Carcass follows a **modular design**, allowing applications to be composed **flexibly** with **independent, reusable components**.

### ☁️ Cloud-Native Support

Built for **modern cloud environments**, Carcass enables **efficient serverless execution**, **distributed computing**, and **seamless scalability**.

### 📐 Domain-Driven Design (DDD)

Carcass is designed around **DDD principles**, providing a **structured** approach to software design, focusing on **business logic** and **clear domain boundaries**.

### ️🔀 Command Query Responsibility Segregation (CQRS)

Separates **command and query responsibilities**, improving **scalability**, **maintainability**, and **performance** in **data-driven applications**.

### 🔁 Event Sourcing

Stores all **state changes** as a **sequence of events**, ensuring **immutability, auditability**, and support for **event-driven architectures**.

### 🏨 Multitenancy

Comprehensive **multitenancy support** allows seamless development of **SaaS applications**, enabling **tenant isolation** and **shared resources**.

### 📝 Audit Logging

Every **system change** is logged to provide **traceability**, **accountability**, and **insights** into **application behavior** over time.

### ⚡ Real-Time Communication

Carcass includes **real-time capabilities** built with **SignalR**, enabling **instant updates** and **interactive experiences**.

### 🔒️ Security

Carcass includes built-in **authentication, authorization**, and **data protection**, with support for **Firebase Authentication**, ensuring **robust security measures**.

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/), [Rider](https://www.jetbrains.com/rider/),
  or [Visual Studio Code](https://code.visualstudio.com/)

### 📥 Installation

To install `Carcass.*` and its dependencies via the .NET Core CLI, execute the following command:

```powershell
dotnet add package Carcass.*
```

To install `Carcass.*` and its dependencies via NuGet, execute the following command:

```powershell
Install-Package Carcass.*
```

### 🗃️ Local NuGet

To pack and publish NuGet packages locally, use the provided `NuGetPackAndPublish.ps1` script. This script automates the process of **building, packing**, and **adding NuGet packages** to your local NuGet repository.

1. Open a **PowerShell terminal** at the root of the project.
2. Run the following command:

   ```powershell
   ./scripts/NuGetPackAndPublish.ps1 -V '<Version>'
   ```

   Replace `<Version>` with the **package version** (e.g., `'1.0.0'`, `'1.0.0-beta1'`). **Note** the single quotes around the version.

3. The packaged files will be saved in the `nupkgs` directory and added to the local **NuGet repository** (`C:/NuGet/packages` by default).

### ⭐ Carcass Samples

#### Carcass in Azure Functions

An example of using the **Carcass framework** with **Azure Functions** can be found in the [`Carcass.Sample.AzureFunctions`](./samples/Carcass.Sample.AzureFunctions/README.md) project. This sample demonstrates how Carcass can be implemented with **Azure's serverless computing model**, including features like dependency injection, Entity Framework Core, Firebase Authentication, and the usage of Azure's `FunctionContext` accessor for working with the function's execution context.

## 📦 Packages

- [Carcass.Core](https://www.nuget.org/packages/Carcass.Core) – Core **abstractions, interfaces, and types** used by **Carcass.\*** libraries.

### Azure

- [Carcass.Azure.Functions](https://www.nuget.org/packages/Carcass.Azure.Functions) – Features for **execution control, state management, and workflow coordination** in **Azure Functions**.

### Data

- [Carcass.Data.Core](https://www.nuget.org/packages/Carcass.Data.Core) – **Domain-Driven Design, CQRS, and Event Sourcing** core **abstractions, interfaces, and types** used by **Carcass.Data.\*** libraries.
- [Carcass.Data.Elasticsearch](https://www.nuget.org/packages/Carcass.Data.Elasticsearch) – **Domain-Driven Design, CQRS, and Event Sourcing** built with **Elasticsearch**.
- [Carcass.Data.EntityFrameworkCore](https://www.nuget.org/packages/Carcass.Data.EntityFrameworkCore) – **Domain-Driven Design, CQRS, and Event Sourcing** built with **Entity Framework Core**.
- [Carcass.Data.EventStoreDb](https://www.nuget.org/packages/Carcass.Data.EventStoreDb) – **Domain-Driven Design, CQRS, and Event Sourcing** built with **EventStoreDB**.
- [Carcass.Data.Firestore](https://www.nuget.org/packages/Carcass.Data.Firestore) – **Domain-Driven Design, CQRS, and Event Sourcing** built with **Firebase Firestore**.
- [Carcass.Data.MongoDb](https://www.nuget.org/packages/Carcass.Data.MongoDb) – **Domain-Driven Design, CQRS, and Event Sourcing** built with **MongoDB**.

### Firebase

- [Carcass.Firebase.Core](https://www.nuget.org/packages/Carcass.Firebase.Core) – **Firebase authentication** core **abstractions, interfaces, and types** used by **Carcass.Firebase.\*** libraries.
- [Carcass.Firebase.AspNetCore](https://www.nuget.org/packages/Carcass.Firebase.AspNetCore) – **Firebase authentication** built with **ASP.NET Core**.
- [Carcass.Firebase.AzureFunctions](https://www.nuget.org/packages/Carcass.Firebase.AzureFunctions) – **Firebase authentication** built with **Azure Functions**.

### JSON

- [Carcass.Json.Core](https://www.nuget.org/packages/Carcass.Json.Core) – **JSON serialization, deserialization, and transformation** core **abstractions, interfaces, and types** used by **Carcass.Json.\*** libraries.
- [Carcass.Json.NewtonsoftJson](https://www.nuget.org/packages/Carcass.Json.NewtonsoftJson) – **JSON serialization, deserialization, and transformation** built with **Newtonsoft.Json**.
- [Carcass.Json.SystemTextJson](https://www.nuget.org/packages/Carcass.Json.SystemTextJson) – **JSON serialization, deserialization, and transformation** built with **System.Text.Json**.

### HTTP

- [Carcass.Http](https://www.nuget.org/packages/Carcass.Http) – **HTTP abstractions** for **ASP.NET Core** applications.

### Logging

- [Carcass.Logging](https://www.nuget.org/packages/Carcass.Logging) – **High-performance logging adapter** built for **.NET Core logging infrastructure**.

### Media

- [Carcass.Media.Core](https://www.nuget.org/packages/Carcass.Media.Core) – **Media file storage, processing, and metadata management** core **abstractions, interfaces, and types** used by **Carcass.Media.\*** libraries.
- [Carcass.Media.AzureBlobs](https://www.nuget.org/packages/Carcass.Media.AzureBlobs) – **Media file storage, processing, and metadata management** built with **Azure Blob Storage**.
- [Carcass.Media.Cloudinary](https://www.nuget.org/packages/Carcass.Media.Cloudinary) – **Media file storage, processing, and metadata management** built with **Cloudinary**.

### Metadata

- [Carcass.Metadata](https://www.nuget.org/packages/Carcass.Metadata) – **Metadata management** for structuring data.

### Multitenancy

- [Carcass.Multitenancy.Core](https://www.nuget.org/packages/Carcass.Multitenancy.Core) – **Tenant isolation, lifecycle management, and resource sharing** core **abstractions, interfaces, and types** used by **Carcass.Multitenancy.\*** libraries.

### Real-Time Communication

- [Carcass.SignalR](https://www.nuget.org/packages/Carcass.SignalR) – **Real-time communication** built with **SignalR**.

### Swagger

- [Carcass.Swashbuckle](https://www.nuget.org/packages/Carcass.Swashbuckle) – **Swagger API documentation** built with **Swashbuckle**.

### YAML

- [Carcass.Yaml.Core](https://www.nuget.org/packages/Carcass.Yaml.Core) – **YAML serialization, deserialization, and transformation** core **abstractions, interfaces, and types** used by **Carcass.Yaml.\*** libraries.
- [Carcass.Yaml.DotNetYaml](https://www.nuget.org/packages/Carcass.Yaml.DotNetYaml) – **YAML serialization, deserialization, and transformation** built with **YamlDotNet**.

## 📜 License

This project is licensed under the [MIT license](LICENSE).