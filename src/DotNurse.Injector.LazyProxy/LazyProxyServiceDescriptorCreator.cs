using LazyProxy;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.LazyProxy;

public class LazyProxyServiceDescriptorCreator : IServiceDescriptorCreator
{
    public ServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        if (!serviceType.IsAbstract)
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime);
        }

        var factory = ActivatorUtilities.CreateFactory(implementationType, Array.Empty<Type>());

        return new ServiceDescriptor(
            serviceType,
            (s) => LazyProxyBuilder.CreateInstance(serviceType, () => factory(s, null)),
            lifetime);
    }
}
