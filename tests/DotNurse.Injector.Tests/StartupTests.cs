using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DotNurse.Injector.Tests.Environment.NamespaceMultiple;

namespace DotNurse.Injector.Tests
{
    public class StartupTests
    {
        [Fact]
        public void AddServicesFrom_ShouldAddCorrectCount_WithSingleInheritance()
        {
            // Arrange
            var services = new ServiceCollection();
            var nameSpace = "Applyze.Utils.DotInjector.Tests.Environment.NamespaceSingle";

            var expected = GetType().Assembly.GetTypes().Where(c => c.Namespace == nameSpace).ToList();

            // Act
            services.AddServicesFrom(nameSpace);

            // Assert
            Assert.Equal(expected.Count, services.Count);
        }

        [Theory]
        [InlineData(ServiceLifetime.Transient)]
        [InlineData(ServiceLifetime.Scoped)]
        [InlineData(ServiceLifetime.Singleton)]
        public void AddServicesFrom_ShouldAddCorrectCount_WithSingleInheritanceAsGivenLifetime(ServiceLifetime lifetime)
        {
            // Arrange
            var services = new ServiceCollection();
            var nameSpace = "Applyze.Utils.DotInjector.Tests.Environment.NamespaceSingle";

            var expected = GetType().Assembly.GetTypes().Where(x => x.Namespace == nameSpace && !x.IsAbstract).ToList();

            // Act
            services.AddServicesFrom(nameSpace, lifetime);

            // Assert
            Assert.Equal(expected.Count, services.Count);

            foreach (var serviceDescriptor in services)
                Assert.Equal(lifetime, serviceDescriptor.Lifetime);

            foreach (var serviceDescriptor in services)
                Assert.Contains(expected, a => a == serviceDescriptor.ImplementationType);
        }

        [Fact]
        public void AddServicesFrom_ShouldAddCorrectCount_WithMultipleInheritance()
        {
            // Arrange
            var services = new ServiceCollection();
            var nameSpace = "Applyze.Utils.DotInjector.Tests.Environment.NamespaceMultiple";

            var expected = GetType().Assembly.GetTypes().Where(x => x.Namespace == nameSpace && !x.IsAbstract).ToList();

            // Act
            services.AddServicesFrom(nameSpace);

            // Assert
            Assert.Equal(expected.Count, services.Count);

            foreach (var serviceDescriptor in services)
                Assert.Contains(expected, a => a == serviceDescriptor.ImplementationType);

            Assert.DoesNotContain(services, a => a.ServiceType == typeof(IVehicle));
        }
    }
}
