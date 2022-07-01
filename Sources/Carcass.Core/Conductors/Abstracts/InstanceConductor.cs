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

using Microsoft.Extensions.Options;

// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Core.Conductors.Abstracts;

public interface IInstanceConductor<out TOptions, out TInstance>
    where TOptions : class
    where TInstance : class
{
    TOptions Options { get; }
    TInstance Instance { get; }
}

public abstract class InstanceConductor<TOptions, TInstance> : IInstanceConductor<TOptions, TInstance>
    where TOptions : class
    where TInstance : class
{
    private readonly Func<TOptions, TInstance>? _factory;
    private TInstance? _instance;
    private TOptions? _options;

    protected InstanceConductor(
        IOptionsMonitor<TOptions> optionsMonitorAccessor,
        Func<TOptions, TInstance>? factory = default
    )
    {
        ArgumentVerifier.NotNull(optionsMonitorAccessor, nameof(optionsMonitorAccessor));

        _factory = factory;

        optionsMonitorAccessor.OnChange(Initialize);
        Initialize(optionsMonitorAccessor.CurrentValue);
    }

    protected InstanceConductor(IOptions<TOptions> optionsAccessor, Func<TOptions, TInstance>? factory = default)
    {
        ArgumentVerifier.NotNull(optionsAccessor, nameof(optionsAccessor));

        _factory = factory;

        Initialize(optionsAccessor.Value);
    }

    public TOptions Options =>
        _options ?? throw new InvalidOperationException($"{nameof(TOptions)} is not initialized.");

    public TInstance Instance =>
        _instance ?? throw new InvalidOperationException($"{nameof(TInstance)} is not initialized.");

    private void Initialize(TOptions options)
    {
        ArgumentVerifier.NotNull(options, nameof(options));

        _options = options;
        _instance = _factory is not null ? _factory(options) : CreateInstance(options);
    }

    protected abstract TInstance? CreateInstance(TOptions options);
}