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

using System.Net;
using AutoMapper;
using Carcass.Azure.Functions.Extensions;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Core.Extensions;
using Carcass.Data.EntityFrameworkCore.Sessions.Abstracts;
using Carcass.Json.Core.Providers.Abstracts;
using Carcass.Logging.Adapters;
using Carcass.Logging.Adapters.Abstracts;
using Carcass.Sample.AzureFunctions.Contracts.Requests.Notes;
using Carcass.Sample.AzureFunctions.Contracts.Requests.Users;
using Carcass.Sample.AzureFunctions.Contracts.Responses.Notes;
using Carcass.Sample.AzureFunctions.Contracts.Responses.Users;
using Carcass.Sample.AzureFunctions.Data.Domain.Documents;
using Carcass.Sample.AzureFunctions.Data.Domain.Users;
using Carcass.Sample.AzureFunctions.Data.Persistence.Extensions;
using FirebaseAdmin;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace Carcass.Sample.AzureFunctions;

public sealed partial class Functions
{
    private readonly IValidator<CreateNoteInput> _createNoteInputValidator;
    private readonly IValidator<CreateUserInput> _createUserInputValidator;
    private readonly FirebaseApp _firebaseApp;
    private readonly IJsonProvider _jsonProvider;
    private readonly LoggerAdapter<Functions> _loggerAdapter;
    private readonly ILoggerAdapterFactory _loggerAdapterFactory;
    private readonly IMapper _mapper;
    private readonly IEntityFrameworkCoreSession _session;

    private readonly TaskOptions _taskOptions = new()
    {
        Retry = new TaskRetryOptions(new RetryPolicy(3, TimeSpan.FromSeconds(3), 2))
    };

    private readonly IUserIdAccessor _userIdAccessor;

    public Functions(
        ILoggerAdapterFactory loggerAdapterFactory,
        IJsonProvider jsonProvider,
        IMapper mapper,
        IValidator<CreateUserInput> createUserInputValidator,
        IValidator<CreateNoteInput> createNoteInputValidator,
        IEntityFrameworkCoreSession session,
        FirebaseApp firebaseApp,
        IUserIdAccessor userIdAccessor)
    {
        ArgumentNullException.ThrowIfNull(loggerAdapterFactory);
        ArgumentNullException.ThrowIfNull(jsonProvider);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(createUserInputValidator);
        ArgumentNullException.ThrowIfNull(createNoteInputValidator);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(firebaseApp);
        ArgumentNullException.ThrowIfNull(userIdAccessor);

        _loggerAdapterFactory = loggerAdapterFactory;
        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<Functions>();
        _jsonProvider = jsonProvider;
        _mapper = mapper;
        _createUserInputValidator = createUserInputValidator;
        _createNoteInputValidator = createNoteInputValidator;
        _session = session;
        _firebaseApp = firebaseApp;
        _userIdAccessor = userIdAccessor;
    }

    [Function(nameof(CreateUser))]
    [AllowAnonymous]
    public async Task<HttpResponseData> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequestData request,
        [DurableClient] DurableTaskClient client)
    {
        HttpResponseData? response;

        try
        {
            string body = await new StreamReader(request.Body).ReadToEndAsync();
            CreateUserInput input = _jsonProvider.Deserialize<CreateUserInput>(body);

            ValidationResult validationResult = await _createUserInputValidator.ValidateAsync(input);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string> validationErrors = validationResult.Errors
                    .ToDictionary(vf => vf.PropertyName.ToLowerFirstLetter()!, vf => vf.ErrorMessage);

                response = request.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync(_jsonProvider.Serialize(_jsonProvider.Serialize(validationErrors)));

                return response;
            }

            string instanceId = await client
                .ScheduleNewOrchestrationInstanceAsync(nameof(CreateUserOrchestrator), input);
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                response = request.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Failed to start orchestration!");

                return response;
            }

            response = await client
                .WaitForCompletionAsync(
                    request,
                    instanceId,
                    TimeSpan.FromSeconds(1),
                    false,
                    true);

            return response;
        }
        catch (Exception e)
        {
            _loggerAdapter.LogError("An error occurred while processing your request.", e);

            response = request.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"An error occurred while processing your request. {e.StackTrace}");

            return response;
        }
    }

    [Function(nameof(GetUser))]
    [Authorize]
    public async Task<HttpResponseData> GetUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")]
        HttpRequestData request,
        [DurableClient] DurableTaskClient client)
    {
        HttpResponseData? response;

        try
        {
            string firebaseId = _userIdAccessor.GetUserId();
            User user = await _session.GetUserByFirebaseId(firebaseId, true);

            CreateDbUserActivityOutput output = _mapper.Map<User, CreateDbUserActivityOutput>(user);

            response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(output);

            return response;
        }
        catch (Exception e)
        {
            _loggerAdapter.LogError("An error occurred while processing your request.", e);

            response = request.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"An error occurred while processing your request. {e.Message}");

            return response;
        }
    }

    [Function(nameof(CreateNote))]
    [Authorize]
    public async Task<HttpResponseData> CreateNote(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notes")]
        HttpRequestData request,
        [DurableClient] DurableTaskClient client)
    {
        HttpResponseData? response;

        try
        {
            string body = await new StreamReader(request.Body).ReadToEndAsync();
            CreateNoteInput input = _jsonProvider.Deserialize<CreateNoteInput>(body);

            ValidationResult validationResult = await _createNoteInputValidator.ValidateAsync(input);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string> validationErrors = validationResult.Errors
                    .ToDictionary(vf => vf.PropertyName.ToLowerFirstLetter()!, vf => vf.ErrorMessage);

                response = request.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync(_jsonProvider.Serialize(_jsonProvider.Serialize(validationErrors)));

                return response;
            }

            string firebaseId = _userIdAccessor.GetUserId();
            User user = await _session.GetUserByFirebaseId(firebaseId, true);

            Note note = new()
            {
                Text = input.Text,
                UserId = user.Id
            };
            await _session.CreateAsync(note);
            await _session.SaveChangesAsync();

            NoteResponse output = _mapper.Map<Note, NoteResponse>(note);

            response = request.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(output);

            return response;
        }
        catch (Exception e)
        {
            _loggerAdapter.LogError("An error occurred while processing your request.", e);

            response = request.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"An error occurred while processing your request. {e.Message}");

            return response;
        }
    }
}