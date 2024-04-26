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

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides utility methods for task execution scenarios.
/// </summary>
public static class ExecutionHelper
{
    // ReSharper disable once AsyncVoidMethod
    /// <summary>
    ///     Executes the given task without awaiting it and handles any exceptions using the provided error action.
    /// </summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="errorAction">
    ///     The action to invoke if an exception occurs during the execution of the task. This parameter
    ///     is optional.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests. If cancellation is requested, an
    ///     exception will be thrown. This parameter is optional.
    /// </param>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public static async void FireAndForget(
        this Task task,
        Action<Exception>? errorAction = null,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await task;
        }
        catch (Exception exception) when (errorAction is not null)
        {
            errorAction(exception);
        }
    }
}