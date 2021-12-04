using System;

namespace DotNurse.Injector.Attributes;

/// <summary>
/// Provides injection from services. This can be used for both Property and field. Used class member must be <see cref="public"/> to be set from outside. Sample usage:
/// <code>
/// [InjectService] public IBookService BookService { get; set; }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class InjectServiceAttribute : Attribute
{
}
