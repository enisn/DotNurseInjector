using DotNurse.Injector.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using DotNurse.Injector.Tests.Sample;
using Xunit;

namespace DotNurse.Injector.Tests.Sample
{
    public interface IDataProvider<T>
    {
        T Get();
    }

    public interface IMessageDataProvider : IDataProvider<string>
    {
    }

    public class MessageDataProvider : IMessageDataProvider
    {
        public string Get()
        {
            return "Hello World!";
        }
    }

    public interface IDataService<T>
    {
        T Retrieve();
    }

    public interface IMessageDataService : IDataService<string>
    {
        IEncryptionService EncryptionService { get; }
    }

    public class MessageDataService : IMessageDataService
    {
        [InjectService]
        public IMessageDataProvider MessageDataProvider { get; set; }

        public IEncryptionService EncryptionService { get; }

        public MessageDataService(IEncryptionService encryptionService)
        {
            this.EncryptionService = encryptionService;
        }

        public string Retrieve()
        {
            return MessageDataProvider.Get();
        }
    }

    public interface IEncryptionService
    {
        public IEncryptionAlgorithm EncryptionAlgorithm { get; set; }
    }

    public class EncryptionService : IEncryptionService
    {
        [InjectService]
        public IEncryptionAlgorithm EncryptionAlgorithm { get; set; }
    }

    public interface IEncryptionAlgorithm
    {
    }

    public class EncryptionAlgorithm : IEncryptionAlgorithm
    {
    }
}

namespace DotNurse.Injector.Tests
{
    public class PropertyInjectionRecursiveTests
    {
        private readonly IServiceProvider serviceProvider;
        public PropertyInjectionRecursiveTests()
        {
            var services = new ServiceCollection();
            services.AddDotNurseInjector();
            services.AddServicesFrom("DotNurse.Injector.Tests.Sample");

            serviceProvider = new DotNurseServiceProvider(services);
        }

        [Fact]
        public void TestCase_1()
        {
            var encryptionService = serviceProvider.GetService<IEncryptionService>();
            Assert.NotNull(encryptionService.EncryptionAlgorithm);
        }

        [Fact]
        public void TestCase_2()
        {
            var service = serviceProvider.GetService<IMessageDataService>();
            Assert.NotNull(service.EncryptionService.EncryptionAlgorithm);
        }
    }
}
