using System;
namespace DotNurse.Injector.Attributes
{
    /// <summary>
    /// Using this attribute prevents registering this object into ServiceCollection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DontRegisterAttribute : Attribute
    {
    }
}

