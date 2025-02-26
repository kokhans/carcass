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

public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("àåáâäãåą", "aaaaaaaa")]
    [InlineData("èéêëę", "eeeee")]
    [InlineData("ìíîïı", "iiiii")]
    [InlineData("òóôõöøőð", "oooooooo")]
    [InlineData("ùúûüŭů", "uuuuuu")]
    [InlineData("çćčĉ", "cccc")]
    [InlineData("żźž", "zzz")]
    [InlineData("śşšŝ", "ssss")]
    [InlineData("ñń", "nn")]
    [InlineData("ýÿ", "yy")]
    [InlineData("ğĝ", "gg")]
    [InlineData("ř", "r")]
    [InlineData("ł", "l")]
    [InlineData("đ", "d")]
    [InlineData("ß", "ss")]
    [InlineData("Þ", "th")]
    [InlineData("ĥ", "h")]
    [InlineData("ĵ", "j")]
    public void GivenString_WhenToAscii_ThenShouldBeAsExpected(string given, string expected)
    {
        // Arrange

        // Act
        string? actual = given.ToAscii();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GivenString_WhenToSnakeCase_ThenShouldBeAsExpected()
    {
        // Arrange
        const string given = "GivenString";
        const string expected = "given_string";

        // Act
        string? actual = given.ToSnakeCase();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GivenSnakeCaseString_WhenToSnakeCase_ThenShouldBeAsExpected()
    {
        // Arrange
        const string given = "snake_case";

        // Act
        string? actual = given.ToSnakeCase();

        // Assert
        actual.ShouldBe(given);
    }

    [Fact]
    public void GivenString_WhenAppendTrailingSlash_ThenShouldBeAsExpected()
    {
        // Arrange
        const string given = "GivenString";
        const string expected = $"{given}/";

        // Act
        string? actual = given.AppendTrailingSlash();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GivenTrailingSlashString_WhenAppendTrailingSlash_ThenShouldBeAsExpected()
    {
        // Arrange
        const string given = "TrailingSlash/";

        // Act
        string? actual = given.AppendTrailingSlash();

        // Assert
        actual.ShouldBe(given);
    }
}