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

public struct ShortGuid
{
    public static readonly ShortGuid Empty = new(Guid.Empty);

    private Guid _guid;
    private string _value;

    public ShortGuid(string value)
    {
        ArgumentVerifier.NotNull(value, nameof(value));

        _value = value;
        _guid = Decode(value);
    }

    public ShortGuid(Guid guid)
    {
        _value = Encode(guid);
        _guid = guid;
    }

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

    public override string ToString() => _value;

    public override bool Equals(object? obj) => obj switch
    {
        ShortGuid shortGuid => Guid.Equals(shortGuid.Guid),
        Guid guid => Guid.Equals(guid),
        string => Guid.Equals(((ShortGuid) obj).Guid),
        _ => false
    };

    public override int GetHashCode() => Guid.GetHashCode();

    public static ShortGuid NewGuid() => new ShortGuid(Guid.NewGuid());

    public static string Encode(string value)
    {
        ArgumentVerifier.NotNull(value, nameof(value));

        Guid guid = new(value);

        return Encode(guid);
    }

    public static string Encode(Guid guid)
    {
        string encoded = Convert.ToBase64String(guid.ToByteArray());
        encoded = encoded
            .Replace("/", "_")
            .Replace("+", "-");

        return encoded[..22];
    }

    public static Guid Decode(string value)
    {
        ArgumentVerifier.NotNull(value, nameof(value));

        value = value
            .Replace("_", "/")
            .Replace("-", "+");
        byte[] buffer = Convert.FromBase64String(value + "==");

        return new Guid(buffer);
    }

    public static bool operator ==(ShortGuid lhs, ShortGuid rhs) => lhs._guid == rhs._guid;

    public static bool operator !=(ShortGuid lhs, ShortGuid rhs) => !(lhs == rhs);

    public static implicit operator string(ShortGuid shortGuid) => shortGuid._value;

    public static implicit operator Guid(ShortGuid shortGuid) => shortGuid._guid;

    public static implicit operator ShortGuid(string value) => new(value);

    public static implicit operator ShortGuid(Guid guid) => new(guid);
}