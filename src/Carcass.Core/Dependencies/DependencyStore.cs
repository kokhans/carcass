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

namespace Carcass.Core.Dependencies;

public sealed class DependencyStore<TDependency> where TDependency : class
{
    private readonly Dictionary<string, TDependency> _dependencies;

    public DependencyStore(Dictionary<string, TDependency>? dependencies = default)
    {
        ArgumentVerifier.NotNull(dependencies, nameof(dependencies));

        _dependencies = dependencies ?? new Dictionary<string, TDependency>();
    }

    public void AddDependency(string name, TDependency dependency)
    {
        ArgumentVerifier.NotNull(name, nameof(name));
        ArgumentVerifier.NotNull(dependency, nameof(dependency));

        _dependencies[name] = dependency;
    }

    public TDependency GetRequiredDependency(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name));

        return _dependencies[name];
    }

    public TDependency? GetOptionalDependency(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name));

        _dependencies.TryGetValue(name, out TDependency? value);

        return value;
    }

    public Dictionary<string, TDependency> GetDependencies() => _dependencies;
}