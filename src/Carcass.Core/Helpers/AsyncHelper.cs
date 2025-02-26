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

using System.Globalization;

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides utility methods for running asynchronous methods in a synchronous context.
/// </summary>
public static class AsyncHelper
{
    /// <summary>
    ///     Provides a factory for creating and configuring <see cref="Task" /> instances with specific options.
    /// </summary>
    /// <remarks>
    ///     This instance of <see cref="TaskFactory" /> is pre-configured with default options:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><see cref="CancellationToken.None" /> for cancellation token.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="TaskCreationOptions.None" /> for task creation options.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="TaskContinuationOptions.None" /> for task continuation options.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="TaskScheduler.Default" /> as the task scheduler.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <threadsafety>
    ///     This factory is thread-safe and can be used concurrently across multiple threads.
    /// </threadsafety>
    private static readonly TaskFactory TaskFactory = new(
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default
    );

    /// <summary>
    ///     Executes an asynchronous function and returns its result in a synchronous context.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the asynchronous function.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The result of the executed asynchronous function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided function is null.</exception>
    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        return TaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Executes an asynchronous method that does not return a value in a synchronous context.
    /// </summary>
    /// <param name="func">The asynchronous method to execute.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided asynchronous method is null.</exception>
    public static void RunSync(Func<Task> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        TaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }
}