using System;
using DotNurse.Injector;
using DotNurse.Injector.LifetimeOwners;
using Microsoft.Extensions.DependencyInjection;

namespace DotNurse.Injector.Extensions;

public static class ServiceProviderExtensions
{
    public static object CreateOwnedInstance(this IServiceProvider serviceProvider, Type instanceType, ServiceLifetime lifetime, params object[] parameters)
    {
        ILifetimeOwner owner;

        if (lifetime == ServiceLifetime.Singleton)
        {
            owner = serviceProvider.GetService<ISingletonOwner>();
        }
        else
        {
            owner = serviceProvider.GetService<IScopedOwner>();
        }

        if (owner == null)
        {
            throw new InvalidOperationException($"ILifetimeOwner couldn't be resolved from IServiceProvider.\n\n - Make sure the IServiceProvider is a {typeof(DotNurseServiceProvider).Name}.");
        }

        var instance = ActivatorUtilities.CreateInstance(serviceProvider, instanceType, parameters);

        owner.TakeOwnership(instance);

        return instance;
    }

    public static void TakeOwnership(this IServiceProvider serviceProvider, object instance, ServiceLifetime lifetime)
    {
        ILifetimeOwner owner;

        if (lifetime == ServiceLifetime.Singleton)
        {
            owner = serviceProvider.GetService<ISingletonOwner>();
        }
        else
        {
            owner = serviceProvider.GetService<IScopedOwner>();
        }

        if (owner == null)
        {
            throw new InvalidOperationException($"ILifetimeOwner couldn't be resolved from IServiceProvider.\n\n - Make sure the IServiceProvider is a {typeof(DotNurseServiceProvider).Name}.");
        }

        owner.TakeOwnership(instance);
    }

    public static void InjectIntoMembers(this IServiceProvider serviceProvider, object instance)
    {
        var attributeInjector = serviceProvider.GetService<IAttributeInjector>() ?? throw new InvalidOperationException($"IAttributeInjector couldn't be resolved from DotNurseServiceProvider.\n\n - Make sure the IServiceProvider is a {typeof(DotNurseServiceProvider).FullName}.");

        attributeInjector.InjectIntoMembers(instance, serviceProvider);
    }
}
