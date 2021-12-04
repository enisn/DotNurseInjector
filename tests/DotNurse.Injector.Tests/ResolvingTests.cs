using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace DotNurse.Injector.Tests;

public class ResolvingTests
{
    private readonly IServiceProvider serviceProvider;

    public interface IMyService<T> { }
    public class MyService<T> : IMyService<T> { }

    public class MyOptions
    {
        public bool IsSomeFeatureEnabled { get; set; }
    }

    public ResolvingTests()
    {
        var services = new ServiceCollection();
        services.AddDotNurseInjector();
        services.AddServicesFrom("DotNurse.Injector.Tests.Sample");

        services.Add(new ServiceDescriptor(typeof(IMyService<>), typeof(MyService<>), ServiceLifetime.Transient));

        services.Configure<MyOptions>(opts => opts.IsSomeFeatureEnabled = true);

        serviceProvider = new DotNurseServiceProvider(services);
    }

    [Fact]
    public void ShouldResolveGenericTypes()
    {
        var service = serviceProvider.GetRequiredService<IMyService<string>>();

        Assert.NotNull(service);
    }

    [Fact]
    public void ShouldResolveIOptions()
    {
        var options = serviceProvider.GetRequiredService<IOptions<MyOptions>>();

        Assert.NotNull(options);
        Assert.True(options.Value.IsSomeFeatureEnabled);
        Assert.True(true);
    }
}
