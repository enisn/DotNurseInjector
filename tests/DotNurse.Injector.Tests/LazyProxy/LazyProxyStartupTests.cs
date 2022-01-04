using DotNurse.Injector.LazyProxy;
using DotNurse.Injector.Tests.Environment.NamespaceAttributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace DotNurse.Injector.Tests.LazyProxy
{
    public class LazyProxyStartupTests
    {
        [Fact]
        public void AddServicesByAttributes_ShouldAddImplementationFactoryForAbstracts_WithLazyProxy()
        {
            var services = new ServiceCollection();

            // Registered Services: typeof(IComputer), typeof(ILaptop), typeof(MyLaptop)

            // Act
            services.AddServicesByAttributes(prefferedCreator: new LazyProxyServiceDescriptorCreator());

            // Assert
            var serviceComputer = services.FirstOrDefault(x => x.ServiceType == typeof(IComputer));

            Assert.Null(serviceComputer.ImplementationType);
            Assert.NotNull(serviceComputer.ImplementationFactory);
        }

        [Fact]
        public void AddServicesByAttributes_ShouldAddImplementationTypeForNonAbstracts_WithLazyProxy()
        {
            // Arrange
            var services = new ServiceCollection();

            // Registered Services: typeof(IComputer), typeof(ILaptop), typeof(MyLaptop)

            // Act
            services.AddServicesByAttributes(prefferedCreator: new LazyProxyServiceDescriptorCreator());

            // Assert
            Assert.Contains(services, x => x.ImplementationType == typeof(MyLaptop) && x.ServiceType == typeof(MyLaptop));

            var serviceComputer = services.FirstOrDefault(x => x.ImplementationType == typeof(MyLaptop));

            Assert.Null(serviceComputer.ImplementationFactory);
            Assert.NotNull(serviceComputer.ImplementationType);
        }

    }
}
