using DotNurse.Injector.Attributes;
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
            defaultProvider = defaultServiceCollection.BuildServiceProvider();
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
