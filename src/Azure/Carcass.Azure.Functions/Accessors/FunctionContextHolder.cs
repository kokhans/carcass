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

namespace Carcass.Azure.Functions.Accessors;

/// <summary>
///     A holder class for managing the current <see cref="FunctionContext" /> in an Azure Function execution.
/// </summary>
/// <remarks>
///     This class is intended to encapsulate and store the <see cref="FunctionContext" /> instance, typically for
///     managing and accessing it across asynchronous execution flows.
/// </remarks>
internal sealed class FunctionContextHolder
{
    /// <summary>
    ///     Represents the execution context of a single Azure Function invocation.
    /// </summary>
    /// <remarks>
    ///     Provides the context for accessing the request, execution, and other metadata related to the
    ///     specific Azure Function being executed.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    ///     Thrown if accessed when the context is not properly initialized or is null.
    /// </exception>
    public FunctionContext? FunctionContext;
}