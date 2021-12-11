using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector.Services;

public class DotNurseTypeExplorer : ITypeExplorer
{
    private IList<Assembly> assemblies;
    private object lockingObj = new object();

    public virtual IEnumerable<Type> FindTypesInNamespace(string @namespace, Assembly assembly = null)
    {
        if (assembly != null)
            return assembly.GetTypes().Where(x => x.Namespace == @namespace && !x.IsAbstract && x.IsClass);

        var assemblies = AssembliesToSearchFor.Where(x => x.FullName.StartsWith(@namespace.Split('.').FirstOrDefault()));

        return assemblies.SelectMany(s => s.GetTypes()).Where(x => x.Namespace == @namespace && !x.IsNested && x.IsClass && !x.IsAbstract);
    }

    public virtual IEnumerable<Type> FindTypesWithAttribute<T>(Assembly assembly = null) where T : Attribute
    {
        if (assembly != null)
            return assembly.GetTypes().Where(x => x.GetCustomAttribute<T>() != null);

        return FindTypesByExpression(x => x.IsDefined(typeof(T)));
    }

    public IEnumerable<Type> FindTypesByExpression(Func<Type, bool> expression, Assembly assembly = null)
    {
        if (assembly != null)
            return assembly.GetTypes().Where(expression);

        return AssembliesToSearchFor.SelectMany(sm => sm.GetTypes()).Where(expression);
    }

    protected IList<Assembly> AssembliesToSearchFor
    {
        get
        {
            if (assemblies == null)
            {
                lock (lockingObj)
                {
                    var _assemblies = Assembly
                            .GetEntryAssembly()
                            .GetReferencedAssemblies()
                            .Select(s => Assembly.Load(s.ToString()))
                            .Concat(new[] { Assembly.GetEntryAssembly() });

                    if (AppDomain.CurrentDomain.FriendlyName == "testhost") // Test host is not referenced directly as a .dll
                    {
                        _assemblies = _assemblies
                            .Concat(AppDomain.CurrentDomain.GetAssemblies());
                    }
                    assemblies = _assemblies.Distinct().ToList();
                }
            }
            return assemblies;
        }
    }
}
