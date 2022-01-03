using DotNurse.Injector.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.Registration;

[DescriptorCreatorName("Default")]
public class DotNurseServiceDescriptorCreator : IServiceDescriptorCreator
{
    public ServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        return new ServiceDescriptor(serviceType, implementationType, lifetime);
    }
}
