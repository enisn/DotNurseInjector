using LazyProxy;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.Registration
{
    public class DotNurseInjectorLazyServiceDescriptorCreator : ILazyServiceDescriptorCreator
    {
        public virtual ServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            var factory = ActivatorUtilities.CreateFactory(implementationType, Array.Empty<Type>());

            return new ServiceDescriptor(
                serviceType,
                (s) => LazyProxyBuilder.CreateInstance(serviceType, () => factory(s, null)),
                lifetime);
        }
    }
}
