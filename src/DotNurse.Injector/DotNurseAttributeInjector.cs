using DotNurse.Injector.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector;

public class DotNurseAttributeInjector : IAttributeInjector
{
    public void InjectIntoMembers(object instance, IServiceProvider serviceProvider)
    {
        var members = instance.GetType().GetMembers().Where(x => x.IsDefined(typeof(InjectServiceAttribute)));
        foreach (var member in members)
        {
            if (member is PropertyInfo propertyInfo)
            {
                var injectedInstance = serviceProvider.GetService(propertyInfo.PropertyType);
                propertyInfo?.SetValue(instance, injectedInstance);
            }

            if (member is FieldInfo fieldInfo)
            {
                var injectedInstance = serviceProvider.GetService(fieldInfo.FieldType);
                fieldInfo?.SetValue(instance, injectedInstance);
            }
        }
    }
}
