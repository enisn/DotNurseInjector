using DotNurse.Injector.Registration;
using DotNurse.Injector.Services;

namespace DotNurse.Injector
{
    public class DotNurseInjectorContext
    {
        public ITypeExplorer TypeExplorer { get; set; } = new DotNurseTypeExplorer();

        public ILazyServiceDescriptorCreator LazyServiceDescriptorCreator { get; set; } = new DotNurseInjectorLazyServiceDescriptorCreator();
    }
}
