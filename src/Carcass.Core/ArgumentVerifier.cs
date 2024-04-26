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

using System.Diagnostics;

// ReSharper disable UnusedMember.Global

namespace Carcass.Core;

/// <summary>
///     Provides utility methods to verify arguments for null values, default values, immutability, and other conditions.
///     Primarily used to validate method and constructor arguments to ensure they meet expected preconditions.
/// </summary>
public static class ArgumentVerifier
{
    /// <summary>
    ///     Verifies that the provided argument is not null. For string arguments, additional validation can check for
    ///     whitespace character content.
    /// </summary>
    /// <typeparam name="T">The type of the argument to verify, constrained to reference types.</typeparam>
    /// <param name="argument">The argument to check for null.</param>
    /// <param name="argumentName">The name of the argument being checked, used in exception messages.</param>
    /// <param name="skipStringWhiteSpace">
    ///     Indicates whether to skip validation for strings containing only whitespace
    ///     characters. Defaults to false.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="argument" /> is null, or if it is a string and
    ///     <paramref name="skipStringWhiteSpace" /> is false while the string is null or consists only of whitespace.
    ///     Also thrown if <paramref name="argumentName" /> is null, empty, or consists only of whitespace.
    /// </exception>
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
        {
            throwException = true;
        }

        if (throwException)
            throw new ArgumentNullException(argumentName);
    }

    /// <summary>
    ///     Verifies that the specified argument is not equal to its default value.
    ///     Throws an exception if the argument is equal to the default value of its type.
    /// </summary>
    /// <typeparam name="T">The type of the argument to verify. Must be a value type.</typeparam>
    /// <param name="argument">The argument value to check.</param>
    /// <param name="argumentName">The name of the argument, used in the exception message.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="argumentName" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="argument" /> is equal to its default value.</exception>
    [DebuggerStepThrough]
    public static void NotDefault<T>(T argument, string argumentName) where T : struct
    {
        NotNull(argumentName, nameof(argumentName));

        if (Equals(argument, default(T)))
            throw new ArgumentException($"{argumentName} cannot have a default value.");
    }

    /// <summary>
    ///     Ensures that the given collection is not read-only.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="argument">The collection to verify.</param>
    /// <param name="argumentName">The name of the argument being verified, for error reporting purposes.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="argumentName" /> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="argument" /> is read-only.</exception>
    [DebuggerStepThrough]
    public static void NotReadOnly<T>(ICollection<T> argument, string argumentName)
    {
        NotNull(argumentName, nameof(argumentName));

        if (argument.IsReadOnly)
            throw new ArgumentException($"{argumentName} cannot be readonly.");
    }

    /// <summary>
    ///     Throws an exception if the provided condition is not met.
    /// </summary>
    /// <param name="expression">A boolean value representing the condition to evaluate. If false, an exception is thrown.</param>
    /// <param name="message">The message to include in the exception if the condition is not met.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="message" /> is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="expression" /> evaluates to false.</exception>
    [DebuggerStepThrough]
    public static void Requires(bool expression, string message)
    {
        NotNull(message, nameof(message));

        if (!expression)
            throw new ArgumentException(message);
    }

    /// <summary>
    ///     Ensures that a given condition is met; otherwise, throws an <see cref="ArgumentException" />.
    /// </summary>
    /// <param name="expression">The condition to verify, represented as a function that returns a boolean.</param>
    /// <param name="message">The error message included in the exception if the condition is not met.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="expression" /> or <paramref name="message" /> is
    ///     null.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown if the provided condition evaluates to false.</exception>
    [DebuggerStepThrough]
    public static void Requires(Func<bool> expression, string message)
    {
        NotNull(expression, nameof(expression));
        NotNull(message, nameof(message));

        if (!expression.Invoke())
            throw new ArgumentException(message);
    }
}