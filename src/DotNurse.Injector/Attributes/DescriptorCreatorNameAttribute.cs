using System;

namespace DotNurse.Injector.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DescriptorCreatorNameAttribute : Attribute
    {
        public string Name { get; set; } = "Default";

        public DescriptorCreatorNameAttribute(string name)
        {
            Name = name;
        }
    }
}
