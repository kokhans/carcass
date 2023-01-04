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

using Carcass.Core.Disposers.Abstracts;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedTypeParameter
// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Core.Conductors.Abstracts;

public interface IInstanceConductor<out TOptions, out TInstance, TInstanceDisposer> : IDisposable
    where TOptions : class
    where TInstance : class
    where TInstanceDisposer : InstanceDisposer<TInstance>
{
    TOptions Options { get; }
    TInstance Instance { get; }
}

public abstract class InstanceConductor<TOptions, TInstance, TInstanceDisposer>
    : Disposable, IInstanceConductor<TOptions, TInstance, TInstanceDisposer>
    where TOptions : class
    where TInstance : class
    where TInstanceDisposer : InstanceDisposer<TInstance>
{
    private readonly Func<TOptions, TInstance>? _factory;
    private TOptions? _options;
    private TInstanceDisposer? _instanceDisposer;

    protected InstanceConductor(
        IOptionsMonitor<TOptions> optionsMonitorAccessor,
        Func<TOptions, TInstance>? factory
    )
    {
        ArgumentVerifier.NotNull(optionsMonitorAccessor, nameof(optionsMonitorAccessor));

        _factory = factory;

        optionsMonitorAccessor.OnChange(Initialize);
        Initialize(optionsMonitorAccessor.CurrentValue);
    }

    protected InstanceConductor(IOptions<TOptions> optionsAccessor, Func<TOptions, TInstance>? factory)
    {
        ArgumentVerifier.NotNull(optionsAccessor, nameof(optionsAccessor));

        _factory = factory;

        Initialize(optionsAccessor.Value);
    }

    public TOptions Options =>
        _options ?? throw new InvalidOperationException($"{nameof(TOptions)} is not initialized.");

    public TInstance Instance
    {
        get
        {
            if (_instanceDisposer is null)
                throw new InvalidOperationException(
                    $"Instance disposer {nameof(TInstanceDisposer)} is not initialized.");

            if (_instanceDisposer.Instance is null)
                throw new InvalidOperationException(
                    $"Instance disposer {nameof(TInstanceDisposer)} instance is not initialized."
                );

            return _instanceDisposer.Instance;
        }
    }

    protected abstract TInstance? CreateInstance(TOptions options);

    private void Initialize(TOptions options)
    {
        ArgumentVerifier.NotNull(options, nameof(options));

        _instanceDisposer?.Dispose();

        _instanceDisposer = (TInstanceDisposer?) Activator.CreateInstance(
            typeof(TInstanceDisposer),
            _factory is not null ? _factory(options) : CreateInstance(options)
        );
        if (_instanceDisposer is null)
            throw new InvalidOperationException($"Instance disposer {nameof(TInstanceDisposer)} is not initialized.");

        _options = options;
    }

    protected override void DisposeManagedResources() => _instanceDisposer?.Dispose();
}