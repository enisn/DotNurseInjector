using DotNurse.Injector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MultiLayeredService.Controllers;
using MultiLayeredService.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiLayeredService
{
    public class MyControllerFactory : IControllerFactory
    {
        private readonly IControllerActivator controllerActivator;
        private readonly IAttributeInjector attributeInjector;

        public MyControllerFactory(IControllerActivator controllerActivator, IAttributeInjector attributeInjector)
        {
            this.controllerActivator = controllerActivator;
            this.attributeInjector = attributeInjector;
        }

        public object CreateController(ControllerContext context)
        {
            var controller = controllerActivator.Create(context);
            attributeInjector.InjectIntoMembers(controller, context.HttpContext.RequestServices);
            return controller;
        }

        public void ReleaseController(ControllerContext context, object controller)
        {
            controllerActivator.Release(context, controller);
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MultiLayeredService", Version = "v1" });
            });

            services.AddServicesFrom("MultiLayeredService.Repositories.Concrete");

            services.AddSingleton<IControllerFactory, MyControllerFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MultiLayeredService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
