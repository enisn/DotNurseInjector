using DotNurse.Injector.Attributes;
using DotNurse.Injector.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector.Registration;

public class DotNurseRegistrationContext
{
    public ITypeExplorer TypeExplorer { get; set; } = new DotNurseTypeExplorer();

    internal Dictionary<string, IServiceDescriptorCreator> DescriptorCreators { get; }

    public DotNurseRegistrationContext()
    {
        DescriptorCreators = TypeExplorer
            .FindTypesByExpression(x => typeof(IServiceDescriptorCreator).IsAssignableFrom(x) && !x.IsAbstract)
            .Distinct()
            .Select(s => new KeyValuePair<string, IServiceDescriptorCreator>(
                key: s.GetCustomAttribute<DescriptorCreatorNameAttribute>()?.Name ?? s.Name,
                value: (IServiceDescriptorCreator)Activator.CreateInstance(s)))
            .ToDictionary(k => k.Key, v => v.Value);
    }
}
