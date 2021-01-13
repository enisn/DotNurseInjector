using DotNurse.Injector.Attributes;
using DotNurse.Injector.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector
{
    public static class Startup
    {
        private static ITypeExplorer TypeExplorer { get; } = new DotNurseTypeExplorer();

        public static IServiceCollection AddServicesFrom(this IServiceCollection services,
                                                            string @namespace,
                                                            ServiceLifetime defaultLifetime = ServiceLifetime.Transient,
                                                            Action<DotNurseInjectorOptions> configAction = null)
        {
            var options = new DotNurseInjectorOptions();
            configAction?.Invoke(options);

            var types = TypeExplorer.FindTypesInNamespace(@namespace, options.Assembly);

            foreach (var type in types)
            {
                if (type.GetCustomAttribute<IgnoreInjectionAttribute>() != null)
                    continue;

                if (!options.SelectImplementtion(type))
                    continue;

                if (options.ImplementationBase != null && options.ImplementationBase.IsAssignableFrom(type))
                    continue;

                var attribute = type.GetCustomAttribute<ServiceLifeTimeAttribute>();
                var lifetime = attribute?.ServiceLifetime ?? defaultLifetime;

                var interfaces = type.GetInterfaces();

                services.Add(new ServiceDescriptor(type, type, lifetime));

                if (interfaces.Length == 1)
                {
                    var inheritFrom = interfaces.FirstOrDefault();
                    services.Add(new ServiceDescriptor(inheritFrom, type, lifetime));

                    continue;
                }

                var registerAsAttribute = type.GetCustomAttributes<RegisterAsAttribute>().ToArray();
                if (registerAsAttribute?.Length > 0)
                {
                    foreach (var injectAsAttribute in registerAsAttribute)
                        if (!services.Any(a => a.ServiceType == injectAsAttribute.ServiceType))
                            services.Add(new ServiceDescriptor(injectAsAttribute.ServiceType, type, injectAsAttribute.ServiceLifetime ?? lifetime));
                    continue;
                }

                if (interfaces.Length > 1)
                {
                    services.Add(new ServiceDescriptor(options.SelectInterface(interfaces), type, lifetime));
                    continue;
                }
            }
            return services;
        }

        /// <summary>
        /// Adds only services which is marked with [<see cref="RegisterAsAttribute"/>] attribute.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServicesByAttributes(this IServiceCollection services, ServiceLifetime defaultServiceLifetime = ServiceLifetime.Transient, Assembly assembly = null)
        {
            var types = TypeExplorer.FindTypesWithAttribute<RegisterAsAttribute>(assembly);

            foreach (var type in types)
                foreach (var injectAsAttribute in type.GetCustomAttributes<RegisterAsAttribute>())
                    services.Add(new ServiceDescriptor(injectAsAttribute.ServiceType, type, injectAsAttribute.ServiceLifetime ?? defaultServiceLifetime));

            return services;
        }

        public static IServiceCollection AddDotNurseInjector(this IServiceCollection services)
        {
            return services;
        }
    }
}
