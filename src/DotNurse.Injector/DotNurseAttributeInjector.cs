using DotNurse.Injector.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DotNurse.Injector;

public class DotNurseAttributeInjector : IAttributeInjector
{
    public void InjectIntoMembers(object instance, IServiceProvider serviceProvider)
    {
        var members = instance.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.IsDefined(typeof(InjectServiceAttribute)));
        foreach (var member in members)
        {
            if (member is PropertyInfo propertyInfo)
            {
                var injectedInstance = serviceProvider.GetRequiredService(propertyInfo.PropertyType);
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
