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

using System.Text.Json.Serialization;

namespace Carcass.Core;

/// <summary>
///     Represents the result of an operation which can either succeed or fail, encapsulating associated data or error
///     information.
/// </summary>
/// <typeparam name="T">The type of the payload contained in the result if the operation succeeds.</typeparam>
public sealed class Result<T>
{
    /// <summary>
    ///     Represents the result of an operation, encapsulating success or failure states with optional payload and failure
    ///     details.
    /// </summary>
    /// <typeparam name="T">The type of the payload associated with a successful result.</typeparam>
    [JsonConstructor]
    private Result(
        T? payload = default,
        string? failureReason = null,
        Exception? failureException = null,
        KeyValuePair<Type, object>? failureData = null
    )
    {
        Payload = payload;
        FailureReason = failureReason;
        FailureException = failureException;
        FailureData = failureData;
    }

    /// <summary>
    ///     Gets the payload of the operation, representing the successful result if the operation completed without errors.
    /// </summary>
    /// <remarks>
    ///     This property holds the value resulting from a successful operation. If the operation fails, this property
    ///     will contain the default value of the type or null, depending on the generic type.
    /// </remarks>
    /// <value>
    ///     The result of the operation if it is successful; otherwise, default or null.
    /// </value>
    public T? Payload { get; }

    /// <summary>
    ///     Gets the reason for the failure in the operation, if applicable.
    /// </summary>
    /// <value>
    ///     A string describing the reason for the failure, or null if the operation was successful.
    /// </value>
    /// <exception cref="ArgumentException">
    ///     Thrown when attempting to set an invalid failure reason.
    /// </exception>
    public string? FailureReason { get; }

    /// <summary>
    ///     Gets the exception object describing the failure that occurred during the operation, if any.
    /// </summary>
    /// <remarks>
    ///     This property holds an exception instance when an operation fails due to an exception.
    ///     It provides detailed information about the error condition, which may be useful for debugging or logging.
    /// </remarks>
    /// <value>An <see cref="Exception" /> instance if the operation failed due to an exception; otherwise, null.</value>
    /// <exception cref="InvalidOperationException">Thrown if invoked in an invalid state of the Result object.</exception>
    public Exception? FailureException { get; }

    /// <summary>
    ///     Represents additional data about the failure, including the type and associated object.
    /// </summary>
    /// <remarks>
    ///     This property can provide extended information about the failure by associating
    ///     the failure with a specific data type and a corresponding object.
    /// </remarks>
    /// <returns>
    ///     A <see cref="KeyValuePair{TKey, TValue}" /> where the key is a <see cref="Type" /> representing
    ///     the type of the failure data and the value is the associated object. Returns null
    ///     if no failure data is specified.
    /// </returns>
    public KeyValuePair<Type, object>? FailureData { get; }

    /// <summary>
    ///     Indicates whether the operation represented by the <see cref="Result{T}" /> instance completed successfully.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the operation succeeded and no failure reason is specified; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSuccess => string.IsNullOrWhiteSpace(FailureReason);

    /// <summary>
    ///     Creates a failed <see cref="Result{T}" /> object with the specified failure details.
    /// </summary>
    /// <typeparam name="T">The type of the payload in the result.</typeparam>
    /// <param name="failureException">The exception that caused the failure, if any.</param>
    /// <param name="failureReason">The reason for the failure, as a string.</param>
    /// <param name="failureType">The type associated with additional failure data.</param>
    /// <param name="failureData">The additional failure data associated with the specified failure type.</param>
    /// <returns>A failed <see cref="Result{T}" /> containing failure details.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when both <paramref name="failureException" /> and
    ///     <paramref name="failureReason" /> are null.
    /// </exception>
    public static Result<T> Fail(
        Exception? failureException = null,
        string? failureReason = null,
        Type? failureType = null,
        object? failureData = null
    )
    {
        ArgumentVerifier.Requires(
            failureException is not null || failureReason is not null,
            $"{nameof(failureException)} and {nameof(failureReason)} must not be null."
        );

        return new Result<T>(
            failureReason: failureReason ?? failureException?.GetBaseException().Message,
            failureException: failureException,
            failureData: failureType is not null && failureData is not null
                ? new KeyValuePair<Type, object>(failureType, failureData)
                : null
        );
    }

    /// <summary>
    ///     Creates a successful result with the specified payload.
    /// </summary>
    /// <typeparam name="T">The type of the payload.</typeparam>
    /// <param name="payload">The payload to encapsulate in the result. Can be null.</param>
    /// <returns>A <see cref="Result{T}" /> object representing a successful operation.</returns>
    public static Result<T> Success(T? payload) => new(payload);
}