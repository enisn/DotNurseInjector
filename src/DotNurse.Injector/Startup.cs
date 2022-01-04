using DotNurse.Injector.Attributes;
using DotNurse.Injector.Registration;
using DotNurse.Injector.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector;

public static class Startup
{
    public static DotNurseRegistrationContext Context = new();
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
        Assembly assembly = null,
        IServiceDescriptorCreator prefferedCreator = null)
    {
        if (prefferedCreator is null)
        {
            // TODO: Find way to configure default for entire context.
            prefferedCreator = Context.DescriptorCreators["Default"];
        }

        var types = Context.TypeExplorer.FindTypesWithAttribute<RegisterAsAttribute>(assembly);

        foreach (var type in types)
            foreach (var attribute in type.GetCustomAttributes<RegisterAsAttribute>())
            {
                var _creator = prefferedCreator;
                if (attribute.DescriptorCreatorName != null && type.IsAbstract)
                    Context.DescriptorCreators.TryGetValue(attribute.DescriptorCreatorName, out _creator);

                services.Add(_creator.Create(attribute.ServiceType, type, attribute.ServiceLifetime ?? defaultServiceLifetime));
            }

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

    public static IServiceCollection AddDotNurseInjector(this IServiceCollection services)
    {
        services.AddSingleton<ITypeExplorer, DotNurseTypeExplorer>();
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

            services.Add(options.ServiceDescriptorCreator.Create(type, type, lifetime));

            if (interfaces.Length == 1)
            {
                var inheritFrom = interfaces.FirstOrDefault();
                services.Add(options.ServiceDescriptorCreator.Create(inheritFrom, type, lifetime));

                continue;
            }

            var registerAsAttribute = type.GetCustomAttributes<RegisterAsAttribute>().ToArray();
            if (registerAsAttribute?.Length > 0)
            {
                foreach (var injectAsAttribute in registerAsAttribute)
                    if (!services.Any(a => a.ServiceType == injectAsAttribute.ServiceType))
                        services.Add(options.ServiceDescriptorCreator.Create(injectAsAttribute.ServiceType, type, injectAsAttribute.ServiceLifetime ?? lifetime));
                continue;
            }

            if (interfaces.Length > 1)
            {
                services.Add(options.ServiceDescriptorCreator.Create(options.SelectInterface(interfaces), type, lifetime));
                continue;
            }
        }

        return services;
    }
}
