using DotNurse.Injector.Attributes;
using DotNurse.Injector.Registration;
using DotNurse.Injector.Services;
using LazyProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector;

public static class Startup
{
    private static DotNurseInjectorContext Context { get; } = new DotNurseInjectorContext();

    public static IServiceCollection AddServicesFrom(this IServiceCollection services,
                                                        string @namespace,
                                                        ServiceLifetime defaultLifetime = ServiceLifetime.Transient,
                                                        Action<DotNurseInjectorOptions> configAction = null)
    {
        var options = new DotNurseInjectorOptions();
        configAction?.Invoke(options);

        var types = Context.TypeExplorer.FindTypesInNamespace(@namespace, options.Assembly);

        services.RegisterTypes(types, defaultLifetime, options);

        return services;
    }

    /// <summary>
    /// Adds only services which is marked with [<see cref="RegisterAsAttribute"/>] attribute.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddServicesByAttributes(
        this IServiceCollection services,
        ServiceLifetime defaultServiceLifetime = ServiceLifetime.Transient,
        bool useLazyProxy = false,
        Assembly assembly = null)
    {
        var types = Context.TypeExplorer.FindTypesWithAttribute<RegisterAsAttribute>(assembly);

        foreach (var type in types)
            foreach (var injectAsAttribute in type.GetCustomAttributes<RegisterAsAttribute>())
                services.Add(CreateServiceDescriptor(injectAsAttribute.ServiceType, type, injectAsAttribute.ServiceLifetime ?? defaultServiceLifetime, useLazyProxy));

        return services;
    }

    public static IServiceCollection AddServicesFrom(
        this IServiceCollection services,
        Func<Type, bool> expression,
        ServiceLifetime defaultServiceLifetime = ServiceLifetime.Transient,
        Action<DotNurseInjectorOptions> configAction = null)
    {
        var options = new DotNurseInjectorOptions();
        configAction?.Invoke(options);

        var types = Context.TypeExplorer.FindTypesByExpression(expression, options.Assembly);

        services.RegisterTypes(types, defaultServiceLifetime, options);
        return services;
    }

    public static IServiceCollection AddDotNurseInjector(this IServiceCollection services, Action<DotNurseInjectorContext> contextAction = null)
    {
        contextAction?.Invoke(Context);
        services.Add(new ServiceDescriptor(typeof(ITypeExplorer), Context.TypeExplorer.GetType(), ServiceLifetime.Singleton));
        services.Add(new ServiceDescriptor(typeof(ILazyServiceDescriptorCreator), Context.LazyServiceDescriptorCreator.GetType(), ServiceLifetime.Transient));
        return services;
    }

    public static IServiceCollection RegisterTypes(
        this IServiceCollection services,
        IEnumerable<Type> types,
        ServiceLifetime defaultLifetime = ServiceLifetime.Transient,
        DotNurseInjectorOptions options = null)
    {
        if (options is null)
        {
            options = new DotNurseInjectorOptions();
        }

        foreach (var type in types)
        {
            if (type.GetCustomAttribute<IgnoreInjectionAttribute>() != null || type.GetCustomAttribute<DontRegisterAttribute>() != null)
                continue;

            if (!options.SelectImplementation(type))
                continue;

            if (options.ImplementationBase != null && options.ImplementationBase.IsAssignableFrom(type))
                continue;

            var attribute = type.GetCustomAttribute<ServiceLifeTimeAttribute>();
            var lifetime = attribute?.ServiceLifetime ?? defaultLifetime;

            var interfaces = type.GetInterfaces();

            services.Add(CreateServiceDescriptor(type, type, lifetime, options.UseLazyProxy));

            if (interfaces.Length == 1)
            {
                var inheritFrom = interfaces.FirstOrDefault();
                services.Add(CreateServiceDescriptor(inheritFrom, type, lifetime, options.UseLazyProxy));

                continue;
            }

            var registerAsAttribute = type.GetCustomAttributes<RegisterAsAttribute>().ToArray();
            if (registerAsAttribute?.Length > 0)
            {
                foreach (var injectAsAttribute in registerAsAttribute)
                    if (!services.Any(a => a.ServiceType == injectAsAttribute.ServiceType))
                        services.Add(CreateServiceDescriptor(injectAsAttribute.ServiceType, type, injectAsAttribute.ServiceLifetime ?? lifetime, options.UseLazyProxy));
                continue;
            }

            if (interfaces.Length > 1)
            {
                services.Add(CreateServiceDescriptor(options.SelectInterface(interfaces), type, lifetime, options.UseLazyProxy));
                continue;
            }
        }

        return services;
    }

    private static ServiceDescriptor CreateServiceDescriptor(
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime = ServiceLifetime.Transient,
        bool withLazyProxy = false)
    {
        if (withLazyProxy && serviceType.IsAbstract)
        {
            return Context.LazyServiceDescriptorCreator.Create(serviceType, implementationType, lifetime);
        }
        else
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime);
        }
    }
}
