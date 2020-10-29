using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.Attributes
{
    /// <summary>
    /// This is a markup to decide serviec type of implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectAsAttribute : Attribute
    {
        /// <summary>
        /// You can define service type to inject this object into.
        /// For example; BookRepository inherits IBookRepository, and it inherit from IBaseRepository too. Object has 2 interface.
        /// You can inject directly to IBaseRepository via using -> [InjectAs(typeof(IBaseRepository))].
        /// That means -> services.AddTransient&lt;IBaseRepository, BookRepository&gt;();
        /// </summary>
        /// <param name="typeToInjectAs"></param>
        public InjectAsAttribute(Type typeToInjectAs)
        {
            this.TypeToInjectAs = typeToInjectAs;
        }

        public InjectAsAttribute(Type typeToInjectAs, ServiceLifetime serviceLifetime) : this(typeToInjectAs)
        {
            this.ServiceLifetime = serviceLifetime;
        }

        public Type TypeToInjectAs { get; set; }

        /// <summary>
        /// Leave it null to use default lifetime.
        /// </summary>
        public ServiceLifetime? ServiceLifetime { get; set; }
    }
}
