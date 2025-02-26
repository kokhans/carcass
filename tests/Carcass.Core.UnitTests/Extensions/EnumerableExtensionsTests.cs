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

using Carcass.Core.Extensions;
using Shouldly;
using Xunit;

namespace Carcass.Core.UnitTests.Extensions;

public sealed class EnumerableExtensionsTests
{
    [Fact]
    public void GivenKeyDoesNotExist_WhenGetOrAdd_ThenShouldAddValue()
    {
        // Given
        const string expectedKey = "key1";
        const int expectedValue = 1;
        Dictionary<string, int> dictionary = new();

        // When
        int actual = dictionary.GetOrAdd(expectedKey, expectedValue);

        // Then
        actual.ShouldBe(expectedValue);
        dictionary.ShouldContainKeyAndValue(expectedKey, expectedValue);
    }

    [Fact]
    public void GivenKeyExists_WhenGetOrAdd_ThenShouldReturnExistingValue()
    {
        // Given
        const string expectedKey = "key1";
        const int existingValue = 1;
        const int newValue = 2;
        Dictionary<string, int> dictionary = new() {{expectedKey, existingValue}};

        // When
        int actual = dictionary.GetOrAdd(expectedKey, newValue);

        // Then
        actual.ShouldBe(existingValue);
        dictionary.ShouldContainKeyAndValue(expectedKey, existingValue);
    }

    [Fact]
    public void GivenKeyExists_WhenTryUpdate_ThenShouldUpdateValue()
    {
        // Given
        const string expectedKey = "key1";
        const int existingValue = 1;
        const int updatedValue = 2;
        Dictionary<string, int> dictionary = new() {{expectedKey, existingValue}};

        // When
        bool actual = dictionary.TryUpdate(expectedKey, updatedValue);

        // Then
        actual.ShouldBeTrue();
        dictionary.ShouldContainKeyAndValue(expectedKey, updatedValue);
    }

    [Fact]
    public void GivenKeyDoesNotExist_WhenTryUpdate_ThenShouldReturnFalse()
    {
        // Given
        const string expectedKey = "key1";
        const int updatedValue = 2;
        Dictionary<string, int> dictionary = new();

        // When
        bool actual = dictionary.TryUpdate(expectedKey, updatedValue);

        // Then
        actual.ShouldBeFalse();
        dictionary.ShouldNotContainKey(expectedKey);
    }
}