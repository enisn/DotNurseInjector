using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.Services
{
    public interface ITypeExplorer
    {
        IEnumerable<Type> FindTypesByExpression(
            Func<Type, bool> expression,
            Assembly assembly = null);

        IEnumerable<Type> FindTypesInNamespace(
            string @namespace,
            Assembly assembly = null);

        IEnumerable<Type> FindTypesWithAttribute<T>(
            Assembly assembly = null) where T : Attribute;
    }
}
