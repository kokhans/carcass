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

using System.Collections.ObjectModel;

namespace Carcass.Data.Core.Aggregates.Abstracts;

public abstract class Aggregate
{
    private readonly List<object> _history;

    protected Aggregate()
    {
        _history = new List<object>();
    }

    public Guid Id { get; set; }
    public long Version { get; set; } = -1;
    public ReadOnlyCollection<object> History => _history.AsReadOnly();

    protected abstract void When(object @event);

    public void Apply(object @event)
    {
        When(@event);

        _history.Add(@event);
    }

    public void Load(long version, IEnumerable<object> history)
    {
        Version = version;

        foreach (object @event in history)
            When(@event);
    }
}