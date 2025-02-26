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

using Xunit;

namespace Carcass.Core.UnitTests;

public sealed class ShortCodeTests
{
    [Fact]
    public void GivenLengthLessThanOrEqualToOne_WhenNewShortCodeCalled_ThenShouldThrowArgumentException()
    {
        // Arrange
        const int invalidLength = 1;

        // Act
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            ShortCode.NewShortCode(invalidLength);
        });

        // Assert
        Assert.Contains("Length must be greater than 0.", exception.Message);
    }

    [Fact]
    public void GivenValidLength_WhenNewShortCodeCalled_ThenShouldReturnStringWithCorrectLength()
    {
        // Arrange
        const int validLength = 10;

        // Act
        string result = ShortCode.NewShortCode(validLength);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(validLength, result.Length);
    }

    [Fact]
    public void GivenValidLength_WhenNewShortCodeCalled_ThenShouldReturnStringWithAllowedCharacters()
    {
        // Arrange
        const int validLength = 15;
        const string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Act
        string result = ShortCode.NewShortCode(validLength);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, c => Assert.Contains(c, allowedCharacters));
    }

    [Fact]
    public void GivenValidLength_WhenNewShortCodeCalledMultipleTimes_ThenShouldReturnUniqueInstances()
    {
        // Arrange
        const int validLength = 8;
        const int iterations = 100;
        string[] shortCodes = new string[iterations];

        // Act
        for (int i = 0; i < iterations; i++) shortCodes[i] = ShortCode.NewShortCode(validLength);

        // Assert
        Assert.Equal(iterations, shortCodes.Distinct().Count());
    }
}