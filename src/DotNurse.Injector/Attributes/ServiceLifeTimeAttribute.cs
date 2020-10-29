using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ServiceLifeTimeAttribute : Attribute
    {
        public ServiceLifeTimeAttribute(ServiceLifetime lifetime)
        {
            this.ServiceLifetime = lifetime;
        }

        public ServiceLifetime ServiceLifetime { get; set; }
    }
}
