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

using Microsoft.Azure.Functions.Worker;

namespace Carcass.Azure.Functions.Accessors.Abstracts;

/// <summary>
///     Provides access to the current <see cref="Microsoft.Azure.Functions.Worker.FunctionContext" />
///     for Azure Functions executions.
/// </summary>
public interface IFunctionContextAccessor
{
    /// <summary>
    ///     Represents the context of an Azure Function execution, providing access to data and environment information
    ///     associated with the current function invocation.
    /// </summary>
    /// <remarks>
    ///     This property is commonly used to store and retrieve the current
    ///     <see cref="Microsoft.Azure.Functions.Worker.FunctionContext" />
    ///     instance for operations requiring contextual information, such as logging, injection handling, or middleware
    ///     processing.
    ///     It is typically used in conjunction with middleware to ensure the correct context is set and accessed.
    /// </remarks>
    /// <value>
    ///     The <see cref="Microsoft.Azure.Functions.Worker.FunctionContext" /> for the current function execution, or
    ///     <c>null</c> if unavailable.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the property is accessed before being properly initialized within the middleware pipeline.
    /// </exception>
    FunctionContext? FunctionContext { get; set; }
}