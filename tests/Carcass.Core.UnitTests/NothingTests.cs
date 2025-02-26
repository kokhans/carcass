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

using Xunit;

namespace Carcass.Core.UnitTests;

public sealed class NothingTests
{
    [Fact]
    public void GivenDefault_WhenAccessNoneProperty_ThenShouldReturnNothingInstance()
    {
        // Act
        Nothing actual = Nothing.None;

        // Assert
        Assert.Equal(new Nothing(), actual);
    }

    [Fact]
    public void GivenMultipleInstances_WhenCompared_ThenShouldBeEqual()
    {
        // Arrange
        Nothing instance1 = new();
        Nothing instance2 = new();

        // Act & Assert
        Assert.Equal(instance1, instance2);
        Assert.True(instance1 == instance2);
    }

    [Fact]
    public void GivenInstance_WhenGetHashCodeCalled_ThenShouldReturnSameHashCodeForEqualInstances()
    {
        // Arrange
        Nothing instance1 = new();
        Nothing instance2 = new();

        // Act
        int hashCode1 = instance1.GetHashCode();
        int hashCode2 = instance2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GivenInstance_WhenToStringCalled_ThenShouldReturnStructName()
    {
        // Arrange
        Nothing nothing = new();

        // Act
        string actual = nothing.ToString();

        // Assert
        Assert.Equal("Nothing", actual);
    }
}