// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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

namespace Carcass.Core.Extensions;

public static class CharExtensions
{
    public static string ToAscii(this char c)
    {
        string s = c.ToString().ToLowerInvariant();

        if ("àåáâäãåą".Contains(s))
            return "a";

        if ("èéêëę".Contains(s))
            return "e";

        if ("ìíîïı".Contains(s))
            return "i";

        if ("òóôõöøőð".Contains(s))
            return "o";

        if ("ùúûüŭů".Contains(s))
            return "u";

        if ("çćčĉ".Contains(s))
            return "c";

        if ("żźž".Contains(s))
            return "z";

        if ("śşšŝ".Contains(s))
            return "s";

        if ("ñń".Contains(s))
            return "n";

        if ("ýÿ".Contains(s))
            return "y";

        if ("ğĝ".Contains(s))
            return "g";

        if (c == 'ř')
            return "r";

        if (c == 'ł')
            return "l";

        if (c == 'đ')
            return "d";

        if (c == 'ß')
            return "ss";

        if (c == 'Þ')
            return "th";

        if (c == 'ĥ')
            return "h";

        if (c == 'ĵ')
            return "j";

        return c.ToString();
    }
}