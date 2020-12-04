using DotNurse.Injector.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.Benchmark.Sample
{
    [RegisterAs(typeof(IDependency), ServiceLifetime.Singleton)]
    [RegisterAs(typeof(IDependencyA))]
    [RegisterAs(typeof(IAnother), ServiceLifetime.Scoped)]
    public class DependencyA : IDependencyA
    {
    }

    [RegisterAs(typeof(IDependencyB))]
    public class DependencyB : IDependencyB
    {
    }

    public class DependencyC : IDependencyC
    {
    }

    public class DependencyD : IDependencyD
    {
    }

    public interface IDependencyA : IDependency
    {
    }

    public interface IDependencyB : IDependency
    {
    }

    public interface IDependencyC : IDependency
    {
    }

    public interface IDependencyD : IDependency
    {
    }

    public interface IDependency
    {
    }

    public interface IAnother
    {
    }
}
