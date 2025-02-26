# Carcass Sample: Azure Functions

The **Carcass.Sample.AzureFunctions** project demonstrates how to use the **Carcass framework** in an
**Azure Functions** environment. This sample provides a comprehensive implementation to build scalable, cloud-native
applications while integrating critical features like authentication, database management, and serverless functions.

## ✨ Features

### 🧩 Dependency Injection

The project demonstrates seamless integration of **dependency injection** with **Azure Functions**, allowing services to
be easily injected and managed across the application.

### 📊 Entity Framework Core

**Entity Framework Core** is used to simplify data access and management, providing robust database interaction
capabilities for structured data storage.

### 🔁 Durable Functions

Workflow creation and task orchestration are implemented using **Azure Durable Functions**, enabling the development of
reliable, stateful workflows.

### 🔒 Firebase Authentication

The sample includes built-in support for **Firebase Authentication**, ensuring secure authentication and authorization
functionality in your applications.

### ⚡ FunctionContext Accessor

Azure's `FunctionContext` is utilized to access function execution details, providing essential context and runtime
information to support dynamic function behaviors.

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/), [Rider](https://www.jetbrains.com/rider/),
  or [Visual Studio Code](https://code.visualstudio.com/)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Docker](https://docs.docker.com/get-started/)
- [Firebase](https://firebase.google.com/docs/get-started)

### ⚙️ Project Configuration

To run this project locally, create a `local.settings.json` file in the Azure Functions project root with the following
configuration:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ConnectionStrings__ApplicationDbContext": "Server=postgres;Port=5432;User ID=postgres;Password=password;Database=carcass_sample_az_functions;",
    "Carcass__Firebase__Json": "<TODO>"
  }
}
```

Replace `<TODO>` with the **service account key** JSON file generated in Firebase Admin for your project.
Refer to the [Firebase documentation](https://firebase.google.com/docs/projects/learn-more#administrative-roles) to
learn how to create a Firebase project and generate a service account key.

In addition, configure the `Carcass__Firebase__Json` appropriately for authentication and interaction with Firebase
services.

### 🛠️ Infrastructure Setup

To set up the infrastructure required for this project, use the provided
script [UpInfrastructure.ps1](../../scripts/UpInfrastructure.ps1). This script
starts a **PostgreSQL** instance preconfigured with the following settings:

- **Host**: `postgres`
- **Port**: `5432`
- **Database**: `carcass_sample_az_functions`
- **User**: `postgres`
- **Password**: `password`

Run the script using PowerShell:

```powershell
cd scripts
./UpInfrastructure.ps1
```

### 🔥 Setting Up Firebase

You need to configure Firebase settings for interactions with the project. Populate the `key` placeholder in the `.http`
file with the necessary details:

1. **Retrieve Web API Key** from your Firebase project settings under the "General" tab.
   Refer to [Firebase documentation](https://firebase.google.com/docs/web/setup) for guidance.

2. Update the `.http` file with your configuration for `@key`.

### 🌐 User Setup and Authentication

1. **Create a New User**: Execute the following request using the `.http` file:
   ```
   POST http://{{hostname}}:{{port}}/api/users
   ```
   Replace `{{hostname}}` and `{{port}}` accordingly (e.g., `localhost` and `7951`).

   Make sure to configure the `email` and `password` variables in your `.http` file before executing the request.

2. **Sign In the User**: Call the Firebase authentication endpoint:
   ```
   POST https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={{key}}
   ```
   Ensure that the `email`, `password`, and `key` variables (retrieved from the **Setting Up Firebase** section) are
   properly set in the `.http` file for authentication.

3. **Get Authenticated User Details**: Validate the user by retrieving the authorized user:
   ```
   GET http://{{hostname}}:{{port}}/api/users
   ```