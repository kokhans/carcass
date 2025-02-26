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

using AutoMapper;
using Carcass.Azure.Functions.Middlewares;
using Carcass.Data.EntityFrameworkCore.Audit;
using Carcass.Data.EntityFrameworkCore.Extensions;
using Carcass.Firebase.AzureFunctions.Middlewares;
using Carcass.Json.SystemTextJson.Settings;
using Carcass.Logging.Adapters;
using Carcass.Logging.Adapters.Abstracts;
using Carcass.Sample.AzureFunctions.Contracts.Requests.Notes;
using Carcass.Sample.AzureFunctions.Contracts.Requests.Users;
using Carcass.Sample.AzureFunctions.Data.Persistence.DbContexts;
using Carcass.Sample.AzureFunctions.Validators.Notes;
using Carcass.Sample.AzureFunctions.Validators.Users;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

builder
    // Carcass.Azure.Functions
    .UseMiddleware<FunctionContextMiddleware>()
    // Carcass.Firebase.AzureFunctions
    .UseMiddleware<FirebaseAzureFunctionsAuthenticationMiddleware>();

builder.Services
    .Configure<LoggerFilterOptions>(lfo =>
        {
            LoggerFilterRule? loggerFilterRule = lfo.Rules
                .FirstOrDefault(lfr => lfr.ProviderName ==
                                       "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (loggerFilterRule is not null)
                lfo.Rules.Remove(loggerFilterRule);

            lfo.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
            lfo.AddFilter("Azure.Storage.Tables", LogLevel.Warning);
            lfo.AddFilter("Azure.Data.Tables", LogLevel.Warning);
            lfo.AddFilter("Azure.Core", LogLevel.Warning);
            lfo.AddFilter("Azure.Identity", LogLevel.Warning);
        }
    )
    .Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

builder.Services
    .AddSingleton(TimeProvider.System)
    .AddMvc();

builder.Services
    // FluentValidation
    .AddScoped<IValidator<CreateUserInput>, CreateUserInputValidator>()
    .AddScoped<IValidator<CreateNoteInput>, CreateNoteInputValidator>()
    // AutoMapper
    .AddAutoMapper(typeof(Program).Assembly)
    // Entity Framework Core
    .AddDbContext<ApplicationDbContext>(
        dcob =>
        {
            dcob.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")!);
            dcob.EnableDetailedErrors();
            dcob.EnableSensitiveDataLogging();
            dcob.UseCarcassAudit(true, JsonColumnType.Jsonb);
        }
    );

builder.Services
    // Carcass.Core
    .AddCarcassNullableCorrelationIdAccessor()
    // Carcass.Json.SystemTextJson
    .AddCarcassSystemTextJsonProvider(SystemTextJsonSettings.Defaults())
    // Carcass.Logging
    .AddCarcassLoggerAdapterFactory()
    // Carcass.Azure.Functions
    .AddCarcassAzureFunctionsContextAccessor()
    // Carcass.Firebase.Core
    .AddCarcassFirebase(builder.Configuration)
    // Carcass.Firebase.AzureFunctions
    .AddCarcassFirebaseAzureFunctionsUserAccessor()
    // Carcass.Data.EntityFrameworkCore
    .AddCarcassEntityFrameworkCoreSession<ApplicationDbContext>()
    // Carcass.Core
    // This method should be the last call in the configuration composition root
    // as it freezes the service collection, preventing further modifications.
    .AddCarcassServiceProviderLocator();

IHost host = builder.Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider serviceProvider = serviceScope.ServiceProvider;

ILoggerAdapterFactory loggerAdapterFactory = serviceProvider.GetRequiredService<ILoggerAdapterFactory>();
LoggerAdapter<Program> loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<Program>();

// Assert AutoMapper types mapping.
IMapper mapper = serviceProvider.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

// Apply migrations.
await using ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
loggerAdapter.LogDebug("Checking for pending migrations...");
IEnumerable<string> pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
loggerAdapter.LogDebug($"Pending migrations: {string.Join(", ", pendingMigrations)}");
loggerAdapter.LogDebug("Applying migrations...");
await dbContext.Database.MigrateAsync();
loggerAdapter.LogDebug("Migrations applied successfully!");

host.Run();