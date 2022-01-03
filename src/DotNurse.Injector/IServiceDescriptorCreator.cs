using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector
{
    public interface IServiceDescriptorCreator
    {
        ServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime);
    }
}
