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

namespace Carcass.Core;

/// <summary>
///     Provides methods to execute asynchronous operations and returns a <see cref="Result{T}" /> object
///     encapsulating the result or the exception if the operation fails.
/// </summary>
public static class ResultExecutor
{
    /// <summary>
    ///     Executes a given asynchronous function and wraps the result in a <see cref="Result{T}" /> object.
    ///     Captures any exceptions thrown during execution and stores them within the result.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the asynchronous function.</typeparam>
    /// <param name="func">The asynchronous function to be executed.</param>
    /// <returns>A <see cref="Result{T}" /> containing either the successful result or information about the failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided function is null.</exception>
    public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> func)
    {
        ArgumentVerifier.NotNull(func, nameof(func));

        try
        {
            return Result<T>.Success(await func.Invoke());
        }
        catch (Exception exception)
        {
            return Result<T>.Fail(exception);
        }
    }
}