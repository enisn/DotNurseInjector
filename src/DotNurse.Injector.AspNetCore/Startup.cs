using DotNurse.Injector;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DotNurse.Injector.AspNetCore;

public static class Startup
{
    /// <summary>
    /// Use <see cref="DotNurseServiceProvider"/> instead of default <see cref="IServiceProvider"/>. It makes possible to use [<see cref="Attributes.InjectServiceAttribute"/>] for properties nad fields instead of constructor injection.
    /// </summary>
    /// <param name="hostBuilder">Default host builder of AspNetCore</param>
    /// <returns>Same <see cref="IHostBuilder"/> which given as parameter.</returns>
    public static IHostBuilder UseDotNurseInjector(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseServiceProviderFactory(new DotNurseServiceProviderFactory());
        hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IControllerFactory, DotNurseControllerFactory>();
            services.AddDotNurseInjector();
        });

        return hostBuilder;
    }
}
