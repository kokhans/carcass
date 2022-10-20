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

using FluentAssertions;
using Xunit;

namespace Carcass.Core.UnitTests;

public sealed class ShortIdTest
{
    [Fact]
    public void GivenGuid_WhenEncoded_ThenShouldBeAsExpectedShortGuidString()
    {
        // Arrange
        Guid given = Guid.Parse("59d7b262-a31a-4e2a-9532-51cde45df7bc");
        const string expected = "YrLXWRqjKk6VMlHN5F33vA";

        // Act
        string actual = ShortGuid.Encode(given);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void GivenShortGuidString_WhenDecode_ThenShouldBeAsExpectedGuid()
    {
        // Arrange
        const string given = "YrLXWRqjKk6VMlHN5F33vA";
        Guid expected = Guid.Parse("59d7b262-a31a-4e2a-9532-51cde45df7bc");

        // Act
        Guid actual = ShortGuid.Decode(given);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void GivenShortGuidsConstructedOnTheSameGuid_WhenEquals_ThenShouldBeTrue()
    {
        // Arrange
        Guid guid = Guid.Parse("59d7b262-a31a-4e2a-9532-51cde45df7bc");
        ShortGuid shortGuid1 = new(guid);
        ShortGuid shortGuid2 = new(guid);

        // Act

        // Assert
        shortGuid1.Equals(shortGuid2).Should().BeTrue();
    }

    [Fact]
    public void GivenShortGuidsConstructedOnTheSameGuid_WhenGetHashCode_ThenShouldBeTheSame()
    {
        // Arrange
        Guid givenGuid = Guid.Parse("59d7b262-a31a-4e2a-9532-51cde45df7bc");
        ShortGuid givenShortGuid1 = new(givenGuid);
        ShortGuid givenShortGuid2 = new(givenGuid);

        // Act

        // Assert
        givenShortGuid1.GetHashCode().Should().Be(givenShortGuid2.GetHashCode());
    }

    [Fact]
    public void GivenShortGuidsConstructedOnDifferentGuids_WhenGetHashCode_ThenShouldNotBeTheSame()
    {
        // Arrange
        ShortGuid givenShortGuid1 = new(Guid.Parse("59d7b262-a31a-4e2a-9532-51cde45df7bc"));
        ShortGuid givenShortGuid2 = new(Guid.Parse("79d7b262-a31a-4e2a-9532-51cde45df7bc"));

        // Act

        // Assert
        givenShortGuid1.GetHashCode().Should().NotBe(givenShortGuid2.GetHashCode());
    }

    [Fact]
    public void GivenShortGuidsCreateOnDifferentGuid_WhenEquals_ThenShouldBeFalse()
    {
        // Arrange
        ShortGuid givenShortGuid1 = new(Guid.Parse("59d7b262-a31a-4e2a-9532-51cde45df7bc"));
        ShortGuid givenShortGuid2 = new(Guid.Parse("79d7b262-a31a-4e2a-9532-51cde45df7bc"));

        // Act

        // Assert
        givenShortGuid1.Equals(givenShortGuid2).Should().BeFalse();
    }
}