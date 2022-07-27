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

using System.Diagnostics;

namespace Carcass.Core;

public static class ArgumentVerifier
{
    [DebuggerStepThrough]
    public static void NotNull<T>(
        T? argument,
        string argumentName,
        bool skipStringWhiteSpace = false
    ) where T : class
    {
        if (string.IsNullOrWhiteSpace(argumentName))
            throw new ArgumentNullException(nameof(argumentName));

        bool throwException = false;

        if (typeof(T) == typeof(string) && skipStringWhiteSpace == false)
        {
            if (string.IsNullOrWhiteSpace(argument as string))
                throwException = true;
        }
        else if (argument is null)
            throwException = true;

        if (throwException)
            throw new ArgumentNullException(argumentName);
    }

    [DebuggerStepThrough]
    public static void NotDefault<T>(T argument, string argumentName) where T : struct
    {
        NotNull(argumentName, nameof(argumentName));

        if (Equals(argument, default(T)))
            throw new ArgumentException($"{argumentName} cannot have a default value.");
    }

    [DebuggerStepThrough]
    public static void NotReadOnly<T>(ICollection<T> argument, string argumentName)
    {
        NotNull(argumentName, nameof(argumentName));

        if (argument.IsReadOnly)
            throw new ArgumentException($"{argumentName} cannot be readonly.");
    }

    [DebuggerStepThrough]
    public static void Requires(bool expression, string message)
    {
        NotNull(message, nameof(message));

        if (!expression)
            throw new ArgumentException(message);
    }

    [DebuggerStepThrough]
    public static void Requires(Func<bool> expression, string message)
    {
        NotNull(message, nameof(message));

        if (!expression.Invoke())
            throw new ArgumentException(message);
    }
}