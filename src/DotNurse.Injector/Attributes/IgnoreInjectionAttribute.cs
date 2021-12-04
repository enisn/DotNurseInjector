using System;

namespace DotNurse.Injector.Attributes
{
    /// <summary>
    /// Use this attribute not to inject this object into ServiceCollection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Obsolete("This attribute is obsolete, use DontRegisterAttribute instead.")]
    public class IgnoreInjectionAttribute : Attribute
    {
    }
}
