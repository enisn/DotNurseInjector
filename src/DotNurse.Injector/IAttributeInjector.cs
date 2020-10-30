using System;

namespace DotNurse.Injector
{
    public interface IAttributeInjector
    {
        void InjectIntoMembers(object instance, IServiceProvider serviceProvider);
    }
}