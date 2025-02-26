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

using Carcass.Core.Helpers;
using Xunit;

namespace Carcass.Core.UnitTests.Helpers;

public sealed class AsyncHelperTests
{
    [Fact]
    public void GivenAsyncMethodWithResult_WhenRunSync_ThenReturnsExpectedValue()
    {
        // Arrange
        const int expected = 1;

        // Act
        int actual = AsyncHelper.RunSync(AsyncMethodWithResult);

        // Assert
        Assert.Equal(expected, actual);
        return;

        async Task<int> AsyncMethodWithResult()
        {
            await Task.Delay(10);

            return expected;
        }
    }

    [Fact]
    public void GivenAsyncVoidTask_WhenRunSync_ThenExecutesSuccessfully()
    {
        // Arrange
        bool wasExecuted = false;

        // Act
        AsyncHelper.RunSync(AsyncVoidTask);

        // Assert
        Assert.True(wasExecuted);
        return;

        async Task AsyncVoidTask()
        {
            await Task.Delay(10);

            wasExecuted = true;
        }
    }

    [Fact]
    public void GivenNullDelegate_WhenRunSync_ThenThrowsArgumentNullException()
    {
        // Arrange
        Func<Task<int>>? nullDelegateWithResult = null;
        Func<Task>? nullDelegate = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AsyncHelper.RunSync(nullDelegateWithResult!));
        Assert.Throws<ArgumentNullException>(() => AsyncHelper.RunSync(nullDelegate!));
    }

    [Fact]
    public void GivenLongRunningTask_WhenRunSync_ThenReturnsExpectedValue()
    {
        // Arrange
        const int expected = 99;

        // Act
        int actual = AsyncHelper.RunSync(LongRunningTask);

        // Assert
        Assert.Equal(expected, actual);
        return;

        async Task<int> LongRunningTask()
        {
            await Task.Delay(100);

            return expected;
        }
    }

    [Fact]
    public void GivenAsyncTaskThatThrowsException_WhenRunSync_ThenPropagatesException()
    {
        // Act & Assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => AsyncHelper.RunSync(ThrowingTask));
        Assert.Equal("Test exception", exception.Message);
        return;

        async Task<int> ThrowingTask()
        {
            await Task.Delay(10);

            throw new InvalidOperationException("Test exception");
        }
    }

    [Fact]
    public void GivenAsyncVoidTaskReplacement_WhenRunSync_ThenExecutesWithoutException()
    {
        // Arrange
        bool wasExecuted = false;

        // Act
        AsyncHelper.RunSync(AsyncVoidTaskReplacement);

        // Assert
        Assert.True(wasExecuted);
        return;

        async Task AsyncVoidTaskReplacement()
        {
            await Task.Delay(10);

            wasExecuted = true;
        }
    }
}