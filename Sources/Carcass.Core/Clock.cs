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

public abstract class Clock
{
    static Clock()
    {
        Current = new DefaultClock();
    }

    public static Clock Current { get; private set; }

    public abstract DateTime Today { get; }
    public abstract DateTime Now { get; }
    public abstract DateTime UtcNow { get; }

    public static void Reset()
    {
        Current = new DefaultClock();
    }

    public static void Set(Clock clock)
    {
        ArgumentVerifier.NotNull(clock, nameof(clock));

        Current = clock;
    }
}

public sealed class DefaultClock : Clock
{
    public override DateTime Today => DateTime.Today;
    public override DateTime Now => DateTime.Now;
    public override DateTime UtcNow => DateTime.UtcNow;
}