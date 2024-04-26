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

using Shouldly;
using Xunit;

namespace Carcass.Core.UnitTests;

public sealed class ResultExecutorTests
{
    [Fact]
    public async Task GivenNullFunc_WhenExecuteAsync_ThenShouldThrowArgumentNullException()
    {
        // Arrange
        Func<Task<string>>? func = null;

        // Act
        ArgumentNullException actual = await Should.ThrowAsync<ArgumentNullException>(async () =>
        {
            await ResultExecutor.ExecuteAsync(func!);
        });

        // Assert
        actual.ParamName.ShouldBe("func");
    }

    [Fact]
    public async Task GivenValidFunc_WhenExecuteAsync_ThenShouldReturnSuccessResult()
    {
        // Arrange
        const string expected = "Success";

        // Act
        Result<string> actual = await ResultExecutor.ExecuteAsync((Func<Task<string>>) Func);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Payload.ShouldBe(expected);
        actual.FailureException.ShouldBeNull();
        return;

        async Task<string> Func() => await Task.FromResult(expected);
    }

    [Fact]
    public async Task GivenExceptionThrowingFunc_WhenExecuteAsync_ThenShouldReturnFailureResult()
    {
        // Arrange
        InvalidOperationException expected = new("Error occurred");

        // Act
        Result<string> actual = await ResultExecutor.ExecuteAsync((Func<Task<string>>) Func);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Payload.ShouldBeNull();
        actual.FailureException.ShouldBe(expected);
        return;

        Task<string> Func() => throw expected;
    }
}