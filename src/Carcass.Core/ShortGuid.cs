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

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Carcass.Core;

/// <summary>
///     Represents a shortened, URL-safe version of a globally unique identifier (GUID).
///     Provides methods for encoding and decoding between standard GUIDs and short, string-based representations.
/// </summary>
public struct ShortGuid : IEquatable<ShortGuid>
{
    /// <summary>
    ///     Represents a predefined, empty instance of the <see cref="ShortGuid" /> type.
    /// </summary>
    public static readonly ShortGuid Empty = new(Guid.Empty);

    /// <summary>
    ///     Represents an internal backing field for storing a <see cref="Guid" /> associated with a <see cref="ShortGuid" />
    ///     instance.
    /// </summary>
    private Guid _guid;

    /// <summary>
    ///     Represents the encoded string value of the current <see cref="ShortGuid" /> instance.
    /// </summary>
    /// <remarks>
    ///     This is a 22-character Base64 encoded representation of a <see cref="Guid" />,
    ///     optimized for compactness and URL safety by replacing certain characters.
    /// </remarks>
    private string _value;

    /// <summary>
    ///     Represents a compressed, short representation of a globally unique identifier (GUID).
    ///     Provides implicit conversions between GUIDs, strings, and ShortGuid, as well as methods for encoding and decoding.
    /// </summary>
    public ShortGuid(string value)
    {
        ArgumentVerifier.NotNull(value, nameof(value));

        _value = value;
        _guid = Decode(value);
    }

    /// <summary>
    ///     Provides a compact, string-representation of a <see cref="Guid" />. This struct offers
    ///     methods for encoding and decoding a <see cref="Guid" /> to and from a short string value.
    /// </summary>
    public ShortGuid(Guid guid)
    {
        _value = Encode(guid);
        _guid = guid;
    }

    /// <summary>
    ///     Gets or sets the <see cref="Guid" /> associated with the instance.
    /// </summary>
    /// <value>
    ///     Represents a globally unique identifier (GUID) value.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if an attempt is made to set the value to null.
    /// </exception>
    public Guid Guid
    {
        get => _guid;
        set
        {
            if (value == _guid)
                return;

            _guid = value;
            _value = Encode(value);
        }
    }

    /// <summary>
    ///     Gets or sets the encoded string representation of the GUID.
    /// </summary>
    /// <value>
    ///     The base64-encoded string form of the GUID without padding characters.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the provided value is null while being set.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown if the provided value is not a valid encoded GUID string.
    /// </exception>
    public string Value
    {
        get => _value;
        set
        {
            if (value == _value)
                return;

            _value = value;
            _guid = Decode(value);
        }
    }

    /// <summary>
    ///     Returns a string representation of the current ShortGuid value.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents the encoded value of the ShortGuid.
    /// </returns>
    public override string ToString() => _value;

    /// <summary>
    ///     Determines whether the current instance is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    ///     True if the current instance is equal to the specified object; otherwise, false.
    /// </returns>
    /// <exception cref="InvalidCastException">
    ///     Thrown when the specified object cannot be cast to <see cref="ShortGuid" />.
    /// </exception>
    public override bool Equals(object? obj) => obj switch
    {
        ShortGuid shortGuid => Guid.Equals(shortGuid.Guid),
        Guid guid => Guid.Equals(guid),
        string => Guid.Equals(((ShortGuid) obj).Guid),
        _ => false
    };

    /// <summary>
    ///     Computes the hash code for the current ShortGuid instance.
    /// </summary>
    /// <returns>
    ///     An integer hash code representing the current ShortGuid instance. The hash code is derived from the underlying
    ///     Guid.
    /// </returns>
    public override int GetHashCode() => Guid.GetHashCode();

    /// <summary>
    ///     Generates a new instance of <see cref="ShortGuid" /> using a new globally unique identifier (GUID).
    /// </summary>
    /// <returns>A new instance of <see cref="ShortGuid" /> wrapping a newly generated GUID.</returns>
    public static ShortGuid NewGuid() => new(Guid.NewGuid());

    /// <summary>
    ///     Encodes a string into a specific format based on the provided GUID value.
    /// </summary>
    /// <param name="value">The input string to encode. This must represent a valid GUID format.</param>
    /// <returns>A string representing the encoded value of the GUID.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="value" /> is null or empty.</exception>
    public static string Encode(string value)
    {
        ArgumentVerifier.NotNull(value, nameof(value));

        Guid guid = new(value);

        return Encode(guid);
    }

    /// <summary>
    ///     Encodes a given <see cref="Guid" /> into a shortened string representation.
    /// </summary>
    /// <param name="guid">The <see cref="Guid" /> to encode.</param>
    /// <returns>A shortened string representation of the specified <see cref="Guid" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown if a null Guid is passed.</exception>
    public static string Encode(Guid guid)
    {
        string encoded = Convert.ToBase64String(guid.ToByteArray());
        encoded = encoded
            .Replace("/", "_")
            .Replace("+", "-");

        return encoded[..22];
    }

    /// <summary>
    ///     Decodes a base64-encoded string representation of a <see cref="Guid" /> into its <see cref="Guid" /> equivalent.
    /// </summary>
    /// <param name="value">
    ///     A base64-encoded string that represents a <see cref="Guid" />.
    ///     All "_" are replaced with "/" and "-" with "+" during decoding.
    /// </param>
    /// <returns>
    ///     Returns the <see cref="Guid" /> equivalent of the provided encoded string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided <paramref name="value" /> is null or empty.
    /// </exception>
    public static Guid Decode(string value)
    {
        ArgumentVerifier.NotNull(value, nameof(value));

        value = value
            .Replace("_", "/")
            .Replace("-", "+");
        byte[] buffer = Convert.FromBase64String(value + "==");

        return new Guid(buffer);
    }

    /// <summary>
    ///     Represents a shorter version of a globally unique identifier (GUID), with functionality for encoding, decoding,
    ///     and converting between a GUID and a base64-encoded string representation.
    /// </summary>
    public static bool operator ==(ShortGuid lhs, ShortGuid rhs) => lhs._guid == rhs._guid;

    /// <summary>
    ///     Represents a shortened version of a GUID, which can be encoded or decoded
    ///     into a standard GUID format or a Base64-compatible string format.
    /// </summary>
    public static bool operator !=(ShortGuid lhs, ShortGuid rhs) => !(lhs == rhs);

    /// <summary>
    ///     Represents a globally unique identifier (GUID) in a shorter, encoded string format.
    /// </summary>
    public static implicit operator string(ShortGuid shortGuid) => shortGuid._value;

    /// <summary>
    ///     Represents a short, Base64-encoded version of a <see cref="System.Guid" />. Provides methods for encoding and
    ///     decoding
    ///     between the short representation and the standard GUID format.
    /// </summary>
    public static implicit operator Guid(ShortGuid shortGuid) => shortGuid._guid;

    /// <summary>
    ///     Represents a compact, base64-encoded version of a <see cref="Guid" />. It provides utility methods for encoding,
    ///     decoding,
    ///     and converting between the traditional GUID and its shorter string representation.
    /// </summary>
    public static implicit operator ShortGuid(string value) => new(value);

    /// <summary>
    ///     Represents a shortened version of a <see cref="Guid" />, encoded as a Base64 string without padding or special
    ///     characters.
    /// </summary>
    public static implicit operator ShortGuid(Guid guid) => new(guid);

    /// <summary>
    ///     Determines whether the current ShortGuid instance is equal to another ShortGuid instance.
    /// </summary>
    /// <param name="other">The ShortGuid instance to compare with the current instance.</param>
    /// <returns>
    ///     true if the current instance is equal to the <paramref name="other" /> instance; otherwise, false.
    /// </returns>
    public bool Equals(ShortGuid other) => _guid.Equals(other._guid) && _value == other._value;
}