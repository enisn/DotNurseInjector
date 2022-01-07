using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector.Services;

public class DotNurseTypeExplorer : ITypeExplorer
{
    public virtual IEnumerable<Type> FindTypesWithAttribute<T>(Assembly assembly = null, bool scanReferences = true) where T : Attribute
    {
        return FindTypesByExpression(x => x.IsDefined(typeof(T)), assembly, scanReferences);
    }

    public IEnumerable<Type> FindTypesByExpression(Func<Type, bool> expression, Assembly assembly = null, bool scanReferences = true)
    {
        assembly ??= Assembly.GetEntryAssembly();

        if (scanReferences)
        {
            return GetAssemblies(assembly).SelectMany(x => x.GetTypes()).Where(expression);
        }

        return assembly.GetTypes().Where(expression);
    }

    private IEnumerable<Assembly> GetAssemblies(Assembly entryAssembly)
    {
        var _assemblies = entryAssembly
                           .GetReferencedAssemblies()
                           .Select(s => Assembly.Load(s.ToString()))
                           .Concat(new[] { entryAssembly });

        if (AppDomain.CurrentDomain.FriendlyName == "testhost") // Test host is not referenced directly as a .dll
        {
            _assemblies = _assemblies
                .Concat(AppDomain.CurrentDomain.GetAssemblies());
        }

        return _assemblies.Distinct();
    }
}
