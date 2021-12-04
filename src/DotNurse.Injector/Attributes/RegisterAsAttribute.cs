using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNurse.Injector.Attributes;

/// <summary>
/// This is a markup to decide serviec type of implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterAsAttribute : Attribute
{
    /// <summary>
    /// You can define service type to inject this object into.
    /// For example; BookRepository inherits IBookRepository, and it inherit from IBaseRepository too. Object has 2 interface.
    /// You can inject directly to IBaseRepository via using -> [InjectAs(typeof(IBaseRepository))].
    /// That means -> services.AddTransient&lt;IBaseRepository, BookRepository&gt;();
    /// </summary>
    /// <param name="serviceType"></param>
    public RegisterAsAttribute(params Type[] serviceTypes)
    {
        this.ServiceTypes = serviceTypes;
    }

    [Obsolete("Use the constructor with array type parameter.")]
    public RegisterAsAttribute(Type serviceType, ServiceLifetime serviceLifetime) : this(serviceType)
    {
        this.ServiceLifetime = serviceLifetime;
    }

    public Type[] ServiceTypes { get; set; }

    /// <summary>
    /// Leave it null to use default lifetime.
    /// </summary>
    public ServiceLifetime? ServiceLifetime { get; set; }
}
