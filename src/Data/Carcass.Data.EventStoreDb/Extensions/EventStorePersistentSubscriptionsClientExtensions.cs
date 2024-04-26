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

using Carcass.Core;
using EventStore.Client;
using Grpc.Core;

namespace Carcass.Data.EventStoreDb.Extensions;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for working with persistent subscriptions on the
///     EventStorePersistentSubscriptionsClient.
/// </summary>
public static class EventStorePersistentSubscriptionsClientExtensions
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Safely creates a persistent subscription with specified settings, optionally recreating it if it already exists.
    /// </summary>
    /// <param name="persistentSubscriptionsClient">
    ///     The instance of <see cref="EventStorePersistentSubscriptionsClient" /> to perform the operation.
    /// </param>
    /// <param name="streamName">
    ///     The name of the stream on which the persistent subscription is to be created. Cannot be null.
    /// </param>
    /// <param name="groupName">
    ///     The name of the group for the persistent subscription. Cannot be null.
    /// </param>
    /// <param name="settings">
    ///     The settings for the persistent subscription. Cannot be null.
    /// </param>
    /// <param name="recreate">
    ///     A boolean value indicating whether to recreate the subscription if it already exists.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation of creating the persistent subscription.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="persistentSubscriptionsClient" />, <paramref name="streamName" />,
    ///     <paramref name="groupName" />, or <paramref name="settings" /> is null.
    /// </exception>
    /// <exception cref="RpcException">
    ///     Thrown when a specific gRPC error occurs during the operation.
    /// </exception>
    public static async Task SafelyCreatePersistentSubscriptionAsync(
        this EventStorePersistentSubscriptionsClient persistentSubscriptionsClient,
        string streamName,
        string groupName,
        PersistentSubscriptionSettings settings,
        bool recreate = false,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(persistentSubscriptionsClient, nameof(persistentSubscriptionsClient));
        ArgumentVerifier.NotNull(streamName, nameof(streamName));
        ArgumentVerifier.NotNull(groupName, nameof(groupName));
        ArgumentVerifier.NotNull(settings, nameof(settings));

        Task createTask = persistentSubscriptionsClient.CreateToStreamAsync(
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
            if (exception.InnerException is not RpcException {Status.StatusCode: StatusCode.AlreadyExists})
                throw;

            if (recreate)
            {
                await persistentSubscriptionsClient.DeleteToStreamAsync(
                    streamName,
                    groupName,
                    cancellationToken: cancellationToken
                );

                await createTask;

                return;
            }

            await persistentSubscriptionsClient.UpdateToStreamAsync(
                streamName,
                groupName,
                settings,
                cancellationToken: cancellationToken
            );
        }
    }
}