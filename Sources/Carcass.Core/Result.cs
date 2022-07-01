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

namespace Carcass.Core;

public sealed class Result<T>
{
    private Result(Exception? failureException, string? failureReason, Type? failureType, object? failureData)
    {
        ArgumentVerifier.Requires(
            failureException is not null || failureReason is not null,
            $"{nameof(failureException)} and {nameof(failureReason)} must not be null."
        );

        if (failureException is not null)
        {
            FailureException = failureException;
            FailureReason = failureException.Message;
        }

        if (!string.IsNullOrWhiteSpace(failureReason))
            FailureReason = failureReason;

        if (failureType is not null && failureData is not null)
            FailureData = new KeyValuePair<Type, object>(failureType, failureData);
    }

    private Result(T? payload) => Payload = payload;

    public T? Payload { get; }
    public string? FailureReason { get; }
    public Exception? FailureException { get; }
    public KeyValuePair<Type, object>? FailureData { get; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(FailureReason);

    public static Result<T> Fail(
        Exception? failureException = default,
        string? failureReason = default,
        Type? failureType = default,
        object? failureData = default
    ) => new(failureException, failureReason, failureType, failureData);

    public static Result<T> Success(T? payload) => new(payload);
}