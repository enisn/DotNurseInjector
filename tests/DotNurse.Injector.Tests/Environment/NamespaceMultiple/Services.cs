namespace DotNurse.Injector.Tests.Environment.NamespaceMultiple
{
    public interface IVehicle
    {
    }

    public interface ICar : IVehicle
    {
    }

    public interface IBike : IVehicle
    {
    }

    public class Bike : IBike
    {
    }

    public class Car : ICar
    {
    }
}
