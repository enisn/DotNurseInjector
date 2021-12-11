using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.Registration
{
    public class DotNurseInjectorLazyServiceDescriptorCreator : ILazyServiceDescriptorCreator
    {
        public virtual ServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime);
        }
    }
}
