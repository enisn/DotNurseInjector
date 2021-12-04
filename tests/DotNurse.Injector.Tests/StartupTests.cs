using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DotNurse.Injector.Tests.Environment.NamespaceMultiple;
using DotNurse.Injector.Tests.Environment.NamespaceAttributes;
using System.Net.Mail;
using DotNurse.Injector.Services;
using Xunit.Abstractions;

namespace DotNurse.Injector.Tests;

public class StartupTests
{
    [Fact]
    public void AddServicesFrom_ShouldAddCorrectCount_WithSingleInheritance()
    {
        // Arrange
        var services = new ServiceCollection();
        var nameSpace = "DotNurse.Injector.Tests.Environment.NamespaceSingle";

        var expected = GetType().Assembly.GetTypes()
            .Where(c => c.Namespace == nameSpace && !c.IsAbstract)
            .SelectMany(sm => new[] { sm, sm.GetInterfaces()?.FirstOrDefault() })
            .Where(x => x != null)
            .ToList();

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
        var nameSpace = "DotNurse.Injector.Tests.Environment.NamespaceSingle";

        var expected = GetType().Assembly.GetTypes()
            .Where(x => x.Namespace == nameSpace && !x.IsAbstract)
            .SelectMany(sm => new[] { sm, sm.GetInterfaces()?.FirstOrDefault() })
            .Where(x => x != null)
            .ToList();

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
        var nameSpace = "DotNurse.Injector.Tests.Environment.NamespaceMultiple";

        var expected = GetType().Assembly.GetTypes()
            .Where(x => x.Namespace == nameSpace && !x.IsAbstract)
            .SelectMany(sm => new[] { sm, sm.GetInterfaces()?.FirstOrDefault() })
            .Where(x => x != null)
            .ToList();

        // Act
        services.AddServicesFrom(nameSpace);

        // Assert
        Assert.Equal(expected.Count, services.Count);

        foreach (var serviceDescriptor in services)
            Assert.Contains(expected, a => a == serviceDescriptor.ImplementationType);

        Assert.DoesNotContain(services, a => a.ServiceType == typeof(IVehicle));
    }

    [Fact]
    public void AddServicesFrom_ShouldAddCorrectCount_WithMultipleAttribute()
    {
        // Arrange
        var services = new ServiceCollection();
        var @namespace = "DotNurse.Injector.Tests.Environment.NamespaceAttributes";

        var expected = new[] { typeof(IComputer), typeof(ILaptop), typeof(MyLaptop) };

        // Act
        services.AddServicesFrom(@namespace);

        // Assert
        Assert.Equal(expected.Length, services.Count);
        foreach (var serviceDescriptor in services)
            Assert.Contains(expected, a => a == serviceDescriptor.ServiceType);
    }

    [Fact]
    public void AddServicesByAttributes_ShouldAddCorrectCount_WithoutParameter()
    {
        // Arrange
        var services = new ServiceCollection();

        var expected = new[] { typeof(IComputer), typeof(ILaptop), typeof(MyLaptop) };

        // Act
        services.AddServicesByAttributes();

        // Assert
        Assert.Equal(expected.Length, services.Count);
        foreach (var serviceDescriptor in services)
            Assert.Contains(expected, a => a == serviceDescriptor.ServiceType);
    }

    [Theory]
    [InlineData(ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton)]
    public void AddServicesByAttributes_ShouldAddCorrectCount_WithLifetimeParameter(ServiceLifetime lifetime)
    {
        // Arrange
        var services = new ServiceCollection();

        var expected = new[] { typeof(IComputer), typeof(ILaptop), typeof(MyLaptop) };

        // Act
        services.AddServicesByAttributes(lifetime);

        // Assert
        Assert.Equal(expected.Length, services.Count);
        foreach (var serviceDescriptor in services)
            Assert.Contains(expected,
                a => a == serviceDescriptor.ServiceType && serviceDescriptor.Lifetime == lifetime);
    }

    [Fact]
    public void AddServicesFrom_RegisterAll_ShouldMatchCount()
    {
        // Arrange
        var services = new ServiceCollection();
        var types = new DotNurseTypeExplorer().FindTypesByExpression(x => true, GetType().Assembly);

        var allTypeCount = types.Count();

        // Act
        services.AddServicesFrom(t => true, configAction: opts => opts.Assembly = GetType().Assembly);

        // Assert
        Assert.True(services.Count >= allTypeCount);
    }

    [Fact]
    public void AddServicesFrom_RegisterRecursive_ShouldMatchCount()
    {
        // Arrange
        var services = new ServiceCollection();
        var types = new DotNurseTypeExplorer()
            .FindTypesByExpression(
                x => x.Namespace?.StartsWith("DotNurse.Injector.Tests.Environment") ?? false,
                GetType().Assembly);

        var allTypesCount = types.Count();

        // Act
        services.AddServicesFrom(x => x.Namespace?.StartsWith("DotNurse.Injector.Tests.Environment") ?? false, configAction: opts => opts.Assembly = GetType().Assembly);

        // Assert
        Console.WriteLine(services.Count + " >= " + allTypesCount);
        Assert.True(services.Count >= types.Count());
    }
}
