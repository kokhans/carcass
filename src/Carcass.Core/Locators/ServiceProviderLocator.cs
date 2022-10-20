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

using Carcass.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Carcass.Core.Locators;

public sealed class ServiceProviderLocator
{
    private static ServiceProviderLocator? _current;

    private readonly ServiceProvider _serviceProvider;

    private ServiceProviderLocator(ServiceProvider serviceProvider)
    {
        ArgumentVerifier.NotNull(serviceProvider, nameof(serviceProvider));

        _serviceProvider = serviceProvider;
    }

    public static ServiceProviderLocator Current
    {
        get
        {
            if (_current is null)
                throw new InvalidOperationException($"{nameof(ServiceProvider)} is not set.");

            return _current;
        }
    }

    public object GetOptionalService(Type serviceType)
    {
        ArgumentVerifier.NotNull(serviceType, nameof(serviceType));

        return _serviceProvider.GetService(serviceType);
    }

    public TService? GetOptionalService<TService>() => _serviceProvider.GetService<TService>();

    public object GetRequiredService(Type serviceType)
    {
        ArgumentVerifier.NotNull(serviceType, nameof(serviceType));

        return _serviceProvider.GetRequiredService(serviceType);
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        TService? service = _serviceProvider.GetService<TService>();
        if (service is not null)
            return service;

        throw new InvalidOperationException(
            $"No required service of type {typeof(TService).Name} is registered."
        );
    }

    public IReadOnlyCollection<TService>? GetOptionalServices<TService>()
    {
        IList<TService> services = _serviceProvider.GetServices<TService>().ToList();

        return services.Any() ? services.AsReadOnlyCollection() : null;
    }

    public IReadOnlyCollection<TService> GetRequiredServices<TService>() where TService : notnull
    {
        IList<TService> services = _serviceProvider.GetServices<TService>().ToList();
        if (services.Any())
            return services.AsReadOnlyCollection();

        throw new InvalidOperationException(
            $"No required services of type {typeof(TService).Name} are registered."
        );
    }

    public static void Set(ServiceProvider serviceProvider)
    {
        ArgumentVerifier.NotNull(serviceProvider, nameof(serviceProvider));

        _current = new ServiceProviderLocator(serviceProvider);
    }
}