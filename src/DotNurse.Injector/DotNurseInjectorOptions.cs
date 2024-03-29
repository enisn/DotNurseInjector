﻿using System;
using System.Linq;
using System.Reflection;

namespace DotNurse.Injector;

public class DotNurseInjectorOptions
{
    /// <summary>
    /// Not required. DotInjector will find your Assembly from your namespace. But you can define it for better performance.
    /// </summary>
    public Assembly Assembly { get; set; }

    /// <summary>
    /// Filter objects by name with given algorithm.
    /// </summary>
    public Func<Type, bool> SelectImplementation { get; set; } = _ => true;

    /// <summary>
    /// If there are multiple interfaces, how to choose. <see cref="InjectAsAttribute"/> is overrides this. Put that attribute over the class. That's it!
    /// </summary>
    public Func<Type[], Type> SelectInterface { get; set; } = interfaces => interfaces.FirstOrDefault();

    /// <summary>
    /// Filters only objects which inherits directly from this type. For ex.: typeof(BaseRepository<>)
    /// </summary>
    public Type ImplementationBase { get; set; }

    /// <summary>
    /// Register the type as self too. True by default.
    /// </summary>
    public bool SelfRegister { get; set; } = true;
}
