using DotNurse.Injector.Attributes;
using DotNurse.Injector.LifetimeOwners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector
{
    public class DotNurseServiceProvider : IServiceProvider, IDisposable
    {
        private readonly IServiceProvider defaultProvider;
        public DotNurseServiceProvider(IServiceCollection defaultServiceCollection)
        {
            var newServices = new ServiceCollection();

            newServices.AddSingleton<ISingletonOwner, LifetimeOwner>();
            newServices.AddScoped<IScopedOwner, LifetimeOwner>();
            newServices.AddSingleton<IAttributeInjector, DotNurseAttributeInjector>();

            foreach (var service in defaultServiceCollection)
            {
                // Using these variables will prevent factory lambda expression from capturing the actual ServiceDescriptors
                var implementationInstance = service.ImplementationInstance;
                var implementationType = service.ImplementationType;
                var implementationFactory = service.ImplementationFactory;
                var lifetime = service.Lifetime;

                if (service.ServiceType.IsGenericType)
                {
                    newServices.Add(service);
                }
                else
                {
                    Func<IServiceProvider, object> factory = serviceProvider =>
                    {
                        object instance;

                        if (implementationInstance != null)
                        {
                            instance = implementationInstance;
                        }
                        else if (implementationType != null)
                        {
                            instance = serviceProvider.CreateOwnedInstance(implementationType, lifetime);
                        }
                        else if (implementationFactory != null)
                        {
                            instance = implementationFactory.Invoke(serviceProvider);
                            serviceProvider.TakeOwnership(instance, lifetime);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        serviceProvider.InjectIntoMembers(instance);

                        return instance;
                    };

                    newServices.Add(new ServiceDescriptor(service.ServiceType, factory, lifetime));
                }
            }

            defaultProvider = newServices.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            var attributeInjector = defaultProvider.GetService<IAttributeInjector>() ?? throw new InvalidOperationException("IAttributeInjector couldn't be resolved from DotNurseServiceProvider.\n\n - Did you add .UseDotNurseInjecor() method at your Program.cs? \n\n - Adding `service.AddDotNurseInjector()` method is enogh to find it.");

            var instance = defaultProvider.GetService(serviceType);

            attributeInjector.InjectIntoMembers(instance, this);

            return instance;
        }

        public void Dispose()
        {
            (defaultProvider as IDisposable)?.Dispose();
        }
    }
}
