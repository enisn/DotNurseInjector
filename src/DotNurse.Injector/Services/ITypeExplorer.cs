using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNurse.Injector.Services;

public interface ITypeExplorer
{
    IEnumerable<Type> FindTypesByExpression(
        Func<Type, bool> expression,
        Assembly assembly = null, bool scanReferences = true);

    IEnumerable<Type> FindTypesWithAttribute<T>(
        Assembly assembly = null, bool scanReferences = true) where T : Attribute;
}
