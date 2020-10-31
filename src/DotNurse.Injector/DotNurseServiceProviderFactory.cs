using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector
{
    public class DotNurseServiceProviderFactory : IServiceProviderFactory<DotNurseServiceProvider>
    {
        public DotNurseServiceProvider CreateBuilder(IServiceCollection services)
        {
            return new DotNurseServiceProvider(services);
        }

        public IServiceProvider CreateServiceProvider(DotNurseServiceProvider containerBuilder)
        {
            return containerBuilder;
        }
    }
}
