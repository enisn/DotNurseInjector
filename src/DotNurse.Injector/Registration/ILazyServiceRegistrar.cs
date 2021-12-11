using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.Registration
{
    public interface ILazyServiceDescriptorCreator
    {
        ServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime);
    }
}
