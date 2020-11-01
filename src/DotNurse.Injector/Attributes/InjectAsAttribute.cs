using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.Attributes
{
    [Obsolete("This property is obsolete. Name doesn't present what it does do. Use [RegisterAs] instead of this [InjectAs].", true)]
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InjectAsAttribute : RegisterAsAttribute
    {
        public InjectAsAttribute(Type serviceType) : base(serviceType)
        {
        }

        public InjectAsAttribute(Type serviceType, ServiceLifetime serviceLifetime) : base(serviceType, serviceLifetime)
        {
        }
        /*
        * !OBSOLETE 
        * Will be removed after next major version
        */
    }
}
