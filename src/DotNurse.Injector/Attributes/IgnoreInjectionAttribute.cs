using System;

namespace DotNurse.Injector.Attributes
{
    /// <summary>
    /// Use this attribute not to inject this object into ServiceCollection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreInjectionAttribute : Attribute
    {
    }
}
