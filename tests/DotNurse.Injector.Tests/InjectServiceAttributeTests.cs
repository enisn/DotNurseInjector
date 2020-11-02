using DotNurse.Injector.Tests.Environment.NamespaceInjectService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DotNurse.Injector.Tests
{
    public class InjectServiceAttributeTests
    {
        [Fact]
        public void InjectServiceByAttribute_ShouldNotBeNullProperty()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDotNurseInjector();
            services.AddServicesFrom("DotNurse.Injector.Tests.Environment.NamespaceInjectService");

            IServiceProvider serviceProvider = new DotNurseServiceProvider(services);

            // Act
            var service = serviceProvider.GetService<IMessageDataService>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<MessageDataService>(service);
            var messageDataProvider = service as MessageDataService;
            Assert.NotNull(messageDataProvider.MessageDataProvider);
        }

        [Fact]
        public void InjectServiceByAttribute_ShouldCallInjectedPropertyMethod()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDotNurseInjector();
            services.AddServicesFrom("DotNurse.Injector.Tests.Environment.NamespaceInjectService");

            IServiceProvider serviceProvider = new DotNurseServiceProvider(services);

            // Act
            var service = serviceProvider.GetService<IMessageDataService>();

            var message = service.Retrieve();

            // Assert
            Assert.NotNull(message);
            Assert.Equal("Hello World!", message);
        }
    }
}
