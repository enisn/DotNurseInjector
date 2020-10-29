using DotNurse.Injector;
using DotNurse.Injector.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class Startup
    {
        public static IServiceCollection AddServicesFrom(this IServiceCollection services,
                                                            string @namespace,
                                                            ServiceLifetime defaultLifetime = ServiceLifetime.Scoped,
                                                            Action<DotNurseInjectorOptions> configAction = null)
        {
            var options = new DotNurseInjectorOptions();
            configAction?.Invoke(options);

            var types = FindTypesInNamespace(@namespace, options.Assembly);

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
                if (interfaces.Length == 0 || options.AddWithoutInterfaceToo)
                {
                    services.Add(new ServiceDescriptor(type, type, lifetime));
                }

                if (interfaces.Length == 1)
                {
                    var inheritFrom = interfaces.FirstOrDefault();
                    services.Add(new ServiceDescriptor(inheritFrom, type, lifetime));

                    continue;
                }

                if (interfaces.Length > 1)
                {
                    var injectAsAttributes = type.GetCustomAttributes<InjectAsAttribute>().ToArray();
                    if (injectAsAttributes?.Length > 0)
                        foreach (var injectAsAttribute in injectAsAttributes)
                            services.Add(new ServiceDescriptor(injectAsAttribute.TypeToInjectAs, type, injectAsAttribute.ServiceLifetime ?? lifetime));
                    else
                        services.Add(new ServiceDescriptor(options.SelectInterface(interfaces), type, lifetime));

                    continue;
                }
            }
            return services;
        }

        private static IEnumerable<Type> FindTypesInNamespace(string @namespace, Assembly assembly = null)
        {
            if (assembly != null)
            {
                return assembly.GetTypes().Where(x => x.Namespace == @namespace && !x.IsAbstract && x.IsClass);
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith(@namespace.Split('.').FirstOrDefault())).ToList();

            return assemblies.SelectMany(s => s.GetTypes()).Where(x => x.Namespace == @namespace && !x.IsNested && x.IsClass && !x.IsAbstract);
        }
    }
}
