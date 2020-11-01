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
                                                            ServiceLifetime defaultLifetime = ServiceLifetime.Transient,
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

                var registerAsAttribute = type.GetCustomAttributes<RegisterAsAttribute>().ToArray();
                if (registerAsAttribute?.Length > 0)
                {
                    foreach (var injectAsAttribute in registerAsAttribute)
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
                    services.Add(new ServiceDescriptor(injectAsAttribute.TypeToInjectAs, type, injectAsAttribute.ServiceLifetime ?? defaultServiceLifetime));
                }

            return services;
        }

        public static IServiceCollection AddDotNurseInjector(this IServiceCollection services)
        {
            services.AddSingleton<IAttributeInjector, DotNurseAttributeInjector>();

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
            var assemblies = Assembly
                    .GetEntryAssembly()
                    .GetReferencedAssemblies()
                    .Select(s => Assembly.Load(s.ToString()))
                    .Concat(new[] { Assembly.GetEntryAssembly() });

            if (AppDomain.CurrentDomain.FriendlyName == "testhost") // Test host is not referenced directly .dll
                assemblies = assemblies
                    .Concat(AppDomain.CurrentDomain.GetAssemblies());

            return assemblies.Distinct();
        }
    }
}
