using DotNurse.Injector.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.Tests.Environment.NamespaceAttributes
{
    public interface IDevice
    {
    }
    public interface IComputer : IDevice
    {
    }
    public interface ICalculator : IDevice
    {
    }
    public interface IMobilePhone : IDevice
    {
    }

    public interface ILaptop : IComputer
    {
    }

    [RegisterAs(typeof(IComputer))]
    [RegisterAs(typeof(ILaptop))]
    [RegisterAs(typeof(MyLaptop))]
    public class MyLaptop : ILaptop
    {
    }
}
