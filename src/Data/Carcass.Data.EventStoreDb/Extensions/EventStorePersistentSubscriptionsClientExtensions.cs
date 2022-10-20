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
using EventStore.Client;
using Grpc.Core;

namespace Carcass.Data.EventStoreDb.Extensions;

public static class EventStorePersistentSubscriptionsClientExtensions
{
    public static async Task SafelyCreatePersistentSubscriptionAsync(
        this EventStorePersistentSubscriptionsClient persistentSubscriptionsClient,
        string streamName,
        string groupName,
        PersistentSubscriptionSettings settings,
        bool recreate = default,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(persistentSubscriptionsClient, nameof(persistentSubscriptionsClient));
        ArgumentVerifier.NotNull(streamName, nameof(streamName));
        ArgumentVerifier.NotNull(groupName, nameof(groupName));
        ArgumentVerifier.NotNull(settings, nameof(settings));
        
        Task createTask = persistentSubscriptionsClient.CreateAsync(
            streamName,
            groupName,
            settings,
            cancellationToken: cancellationToken
        );

        try
        {
            await createTask;
        }
        catch (Exception exception)
        {
            if (exception.InnerException is RpcException {Status.StatusCode: StatusCode.AlreadyExists})
            {
                if (recreate)
                {
                    await persistentSubscriptionsClient.DeleteAsync(
                        streamName,
                        groupName,
                        cancellationToken: cancellationToken
                    );

                    await createTask;

                    return;
                }

                await persistentSubscriptionsClient.UpdateAsync(
                    streamName,
                    groupName,
                    settings,
                    cancellationToken: cancellationToken
                );

                return;
            }

            throw;
        }
    }
}