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

using Carcass.Azure.Functions.Accessors.Abstracts;
using Microsoft.Azure.Functions.Worker;

namespace Carcass.Azure.Functions.Accessors;

/// <summary>
///     Maintains and provides access to the current <see cref="Microsoft.Azure.Functions.Worker.FunctionContext" />
///     for Azure Functions execution within the context of an asynchronous operation.
/// </summary>
/// <remarks>
///     This class uses an <see cref="System.Threading.AsyncLocal{T}" /> implementation to store the function context
///     specific to the caller's execution context, ensuring thread safety and proper state management across asynchronous
///     operations.
/// </remarks>
public sealed class FunctionContextAccessor : IFunctionContextAccessor
{
    /// <summary>
    ///     An <see cref="AsyncLocal{T}" /> field that holds the current <see cref="FunctionContextHolder" /> instance
    ///     for managing and accessing the Azure Function execution context.
    /// </summary>
    /// <remarks>
    ///     This is used internally to store and retrieve the <see cref="FunctionContext" /> in an
    ///     asynchronous and thread-local manner.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    ///     Thrown when attempting to access the holder value and it is not initialized.
    /// </exception>
    private static readonly AsyncLocal<FunctionContextHolder> Holder = new();

    /// <summary>
    ///     Represents the current execution context of an Azure Function that allows access to
    ///     runtime details during the function's lifecycle.
    /// </summary>
    /// <remarks>
    ///     The <c>FunctionContext</c> is managed via an asynchronous-local storage mechanism to
    ///     ensure proper scoping across individual function executions and independent
    ///     requests. It is used to retrieve and store runtime details of Azure Functions
    ///     during their execution, providing necessary information for middleware and other
    ///     infrastructure layers.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if attempting to manipulate the <c>FunctionContext</c> without proper initialization
    ///     during the function's lifecycle.
    /// </exception>
    public FunctionContext? FunctionContext
    {
        get => Holder.Value?.FunctionContext;
        set
        {
            FunctionContextHolder? holder = Holder.Value;
            if (holder is not null)
            {
                // Clear the current context stored in AsyncLocal, as the operation is complete.
                holder.FunctionContext = null;
            }

            if (value is not null)
            {
                // Use an object wrapper to manage the context in AsyncLocal,
                // allowing it to be consistently cleared across all ExecutionContexts.
                Holder.Value = new FunctionContextHolder {FunctionContext = value};
            }
        }
    }
}