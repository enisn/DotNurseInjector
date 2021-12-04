using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNurse.Injector.AspNetCore;

public class DotNurseControllerFactory : IControllerFactory
{
    private readonly IControllerActivator controllerActivator;
    private readonly IAttributeInjector attributeInjector;

    public DotNurseControllerFactory(IControllerActivator controllerActivator, IAttributeInjector attributeInjector)
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
