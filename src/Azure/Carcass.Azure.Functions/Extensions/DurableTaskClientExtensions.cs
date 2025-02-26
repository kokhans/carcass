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
using Carcass.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace Carcass.Azure.Functions.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="Microsoft.DurableTask.Client.DurableTaskClient" /> class to handle
///     durable task operations more effectively.
/// </summary>
public static class DurableTaskClientExtensions
{
    /// <summary>
    ///     Waits for a specified durable task instance to complete, fail, or terminate, polling its status at a specified
    ///     interval.
    ///     Generates an HTTP response indicating the task's current status or completion outcome.
    /// </summary>
    /// <param name="client">The <see cref="DurableTaskClient" /> used to interact with the orchestration instance.</param>
    /// <param name="request">The HTTP request data used to create a response.</param>
    /// <param name="instanceId">The unique identifier for the orchestration instance being monitored.</param>
    /// <param name="retryInterval">
    ///     The time interval to wait between polling the orchestration instance status. Defaults to one second if not
    ///     specified.
    /// </param>
    /// <param name="returnInternalServerErrorOnFailure">
    ///     Indicates whether to return an internal server error status if the orchestration instance fails. Defaults to false.
    /// </param>
    /// <param name="getInputsAndOutputs">
    ///     Specifies whether the inputs and outputs of the orchestration instance should be retrieved. Defaults to false.
    /// </param>
    /// <param name="cancellation">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the orchestration instance to complete.
    /// </param>
    /// <returns>
    ///     An <see cref="HttpResponseData" /> that represents the HTTP response with the orchestration's current status or
    ///     outcome.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="client" />, <paramref name="request" />, or <paramref name="instanceId" /> is null or
    ///     empty.
    /// </exception>
    public static async Task<HttpResponseData> WaitForCompletionAsync(
        this DurableTaskClient client,
        HttpRequestData request,
        string instanceId,
        TimeSpan? retryInterval = null,
        bool returnInternalServerErrorOnFailure = false,
        bool getInputsAndOutputs = false,
        CancellationToken cancellation = default
    )
    {
        ArgumentVerifier.NotNull(client, nameof(client));
        ArgumentVerifier.NotNull(request, nameof(request));
        ArgumentVerifier.NotNull(instanceId, nameof(instanceId));

        TimeSpan retryIntervalLocal = retryInterval ?? TimeSpan.FromSeconds(1);
        try
        {
            while (true)
            {
                OrchestrationMetadata? metadata =
                    await client.GetInstanceAsync(instanceId, getInputsAndOutputs, cancellation);
                if (metadata?.RuntimeStatus is OrchestrationRuntimeStatus.Completed or
                    OrchestrationRuntimeStatus.Terminated or
                    OrchestrationRuntimeStatus.Failed)
                {
                    HttpResponseData response = request.CreateResponse(
                        metadata.RuntimeStatus == OrchestrationRuntimeStatus.Failed &&
                        returnInternalServerErrorOnFailure
                            ? HttpStatusCode.InternalServerError
                            : HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(new
                    {
                        metadata.Name,
                        metadata.InstanceId,
                        metadata.CreatedAt,
                        metadata.LastUpdatedAt,
                        RuntimeStatus = metadata.RuntimeStatus.ToString(),
                        metadata.SerializedInput,
                        metadata.SerializedOutput,
                        metadata.SerializedCustomStatus
                    }, cancellation);

                    return response;
                }

                await Task.Delay(retryIntervalLocal, cancellation);
            }
        }
        catch (OperationCanceledException)
        {
            return await client
                .CreateCheckStatusResponseAsync(request, instanceId, HttpStatusCode.Accepted, cancellation);
        }
    }
}