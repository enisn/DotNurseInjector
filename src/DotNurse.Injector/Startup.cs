using DotNurse.Injector;
using DotNurse.Injector.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
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
                    services.Add(new ServiceDescriptor(options.SelectInterface(interfaces), type, lifetime));
                    continue;
                }

                var injectAsAttributes = type.GetCustomAttributes<InjectAsAttribute>().ToArray();
                if (injectAsAttributes?.Length > 0)
                    foreach (var injectAsAttribute in injectAsAttributes)
                        services.Add(new ServiceDescriptor(injectAsAttribute.TypeToInjectAs, type, injectAsAttribute.ServiceLifetime ?? lifetime));
            }
            return services;
        }

        /// <summary>
        /// Adds only services which is marked with [<see cref="InjectAsAttribute"/>] attribute.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServicesByAttributes(this IServiceCollection services, ServiceLifetime defaultServiceLifetime = ServiceLifetime.Transient)
        {
            var types = FindTypesWithAttribute<InjectAsAttribute>();

            foreach (var type in types)
                foreach (var injectAsAttribute in type.GetCustomAttributes<InjectAsAttribute>())
                {
                    services.Add(new ServiceDescriptor(injectAsAttribute.TypeToInjectAs,type, injectAsAttribute.ServiceLifetime ?? defaultServiceLifetime));
                }
            return services;
        }
        private static IEnumerable<Type> FindTypesInNamespace(string @namespace, Assembly assembly = null)
        {
            if (assembly != null)
            {
                return assembly.GetTypes().Where(x => x.Namespace == @namespace && !x.IsAbstract && x.IsClass);
            }

            var assemblies = GetAssembliesToSearchFor().Where(x => x.FullName.StartsWith(@namespace.Split('.').FirstOrDefault())).ToList();
            
            return assemblies.SelectMany(s => s.GetTypes()).Where(x => x.Namespace == @namespace && !x.IsNested && x.IsClass && !x.IsAbstract);
        }

        private static IEnumerable<Type> FindTypesWithAttribute<T>(Assembly assembly = null) where T : Attribute
        {
            if (assembly == null)
            {
                return assembly.GetTypes().Where(x => x.GetCustomAttribute<T>() != null);
            }
            return GetAssembliesToSearchFor().SelectMany(sm => sm.GetTypes()).Where(x => x.GetCustomAttribute<T>() != null);
        }

        private static IEnumerable<Assembly> GetAssembliesToSearchFor()
        {
            return Assembly
                    .GetEntryAssembly()
                    .GetReferencedAssemblies()
                    .Select(s => Assembly.Load(s.ToString()))
                    .Concat(new[] { Assembly.GetEntryAssembly() });
        }
    }
}
