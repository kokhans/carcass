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

using Carcass.Logging.Adapters;
using Carcass.Sample.AzureFunctions.Contracts.Requests.Users;
using Carcass.Sample.AzureFunctions.Contracts.Responses.Users;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace Carcass.Sample.AzureFunctions;

public sealed partial class Functions
{
    [Function(nameof(CreateUserOrchestrator))]
    public async Task<CreateDbUserActivityOutput> CreateUserOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        CreateUserInput input)
    {
        ILogger safeLogger = context.CreateReplaySafeLogger<Functions>();
        LoggerAdapter safeLoggerAdapter = _loggerAdapterFactory.CreateLoggerAdapter(safeLogger);

        safeLoggerAdapter.LogDebug("Creating user in Firebase.");
        string firebaseId = await context.CallActivityAsync<string>(nameof(CreateFirebaseUserActivity),
            _mapper.Map<CreateUserInput, CreateFirebaseUserActivityInput>(input), _taskOptions);

        safeLoggerAdapter.LogDebug("Creating user in database.");
        CreateDbUserActivityOutput output = await context.CallActivityAsync<CreateDbUserActivityOutput>(
            nameof(CreateDbUserActivity),
            _mapper.Map<CreateUserInput, CreateDbUserActivityInput>(input,
                moo => { moo.Items.Add("FirebaseId", firebaseId); }), _taskOptions);

        return output;
    }
}