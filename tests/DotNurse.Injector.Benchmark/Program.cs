using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace DotNurse.Injector.Benchmark;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<MyTestStartup>();
    }
}

[SimpleJob(RuntimeMoniker.HostProcess)]
[RPlotExporter]
[MemoryDiagnoser]
public class MyTestStartup
{
    IServiceCollection services;
    [GlobalSetup]
    public void Setup()
    {
        services = new ServiceCollection();
    }

    [Benchmark]
    public void AddByAttributesWithAssembly()
    {
        services.AddServicesByAttributes(assembly: GetType().Assembly);
    }

    [Benchmark]
    public void AddByAttributes()
    {
        services.AddServicesByAttributes();
    }

    [Benchmark]
    public void AddByNamespace()
    {
        services.AddServicesFrom("DotNurse.Injector.Benchmark.Sample");
    }

    [Benchmark]
    public void AddByNamespaceWithAssembly()
    {
        services.AddServicesFrom("DotNurse.Injector.Benchmark.Sample", configAction: opts => opts.Assembly = GetType().Assembly);
    }
}
