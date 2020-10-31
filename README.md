 # DotNurseInjector


<table border="0">
<tr>
<td> <img width="95" src="https://raw.githubusercontent.com/enisn/DotNurseInjector/master/art/dotnurse-icon.png" /> </td>

<td>
<h2>.Nurse Injector</h2>
Simple, lightweight & useful Dependency Injector for dotnet.
 </td>
</tr>
</table>


![.NET Core](https://github.com/enisn/DotNurseInjector/workflows/.NET%20Core/badge.svg)
<a href="https://www.nuget.org/packages/DotNurse.Injector.AspNetCore/">
    <img alt="Nuget" src="https://img.shields.io/nuget/v/DotNurse.Injector.AspNetCore?logo=nuget&style=flat-square">
</a>
<a href="https://www.codefactor.io/repository/github/enisn/dotnurseinjector">
<img src="https://www.codefactor.io/repository/github/enisn/dotnurseinjector/badge" alt="CodeFactor" />
</a>
<a href="https://gitmoji.carloscuesta.me">
  <img src="https://img.shields.io/badge/gitmoji-%20😜%20😍-FFDD67.svg?style=flat-square" alt="Gitmoji">
</a>

---

## Getting Started

- Install Nuget package [from here](https://www.nuget.org/packages/DotNurse.Injector.AspNetCore/).

- Go your **Startup.cs**, remove all your manual injections and use `AddServicesFrom()` method with namespace.

  - If you have following pattern:
```csharp

services.AddTransient<IBookRepository, BookRepository>();
services.AddTransient<IAuthorRepository, AuthorRepository>();
services.AddTransient<IPublisherRepository, PublisherRepository>();
//...
```
  
  - Replace them with following:

```csharp
services.AddServicesFrom("MyCompany.ProjectName.Repositories.Concrete"); // <-- Your implementations namespace.

```

- That's it! DotNurse can find your namespace from any assembly. You don't need to send any Assembly parameter.


***

## Property/Field Injection
> This section is **optional**. You can still use default Microsoft Dependency Injection and skip this step.

You can use attribute injection instead of constructor injection. Using `[InjectService]` attribute for properties or fields is enough to inject them from IoC.

### Setting Up

You must replace your Service Provider with .Nurse Injecor to use **Attribute Injection**.

- Go your **Program.cs** and add `UseDotNurseInjector()` method to IHostBuilder.

```csharp
 public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDotNurseInjector() // <-- Adding this one is enough!
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```


### Usage
```csharp
[InjectService] public IBookRepository BookRepository { get; set; }
```

```csharp
[InjectService] public IBookRepository bookRepository;
```


***

<img src="https://raw.githubusercontent.com/enisn/DotNurseInjector/master/art/dotnurse-github.png" alt="dotnurse-injector-social-preview" />

***

# Customizations

DotNurse meets your custom requirements such as a defining lifetime, injecting into different interfaces etc.

***

## Managing from Startup

.Nurse provides fluent api to manage your injections from single point.

### Service Lifetime

```csharp
services.AddServicesFrom("MyCompany.ProjectName.Services", ServiceLifetime.Scoped);
```

### Interface Selector
You can define a pattern to choose interface from multiple interfaces.

```csharp
services.AddServicesFrom("ProjectNamespace.Services", ServiceLifetime.Scoped, opts =>
{
    opts.SelectInterface = interfaces => interfaces.FirstOrDefault(x => x.Name.EndsWith("Repository"));
});
```

### Implementation Selector
You can define a pattern to choose which objects will be injected into IoC. For example you have a base type in same namespace and you don't want to add it into service collection. You can use this feature:

- ProjectNamespace.Services
  - BookService
  - ~~BaseService~~  <- You want to ignore this
  - AuthorService
  - ...
 
```csharp
services.AddServicesFrom("ProjectNamespace.Services", ServiceLifetime.Scoped, opts =>
{
    opts.SelectImplementtion = i => !i.Name.StartsWith("Base");
});
```

### Implementation Base
Think about same scenario like previous, you can choose a base type to inject all classes which is inhetired from.

```csharp
services.AddServicesFrom("ProjectNamespace.Services", ServiceLifetime.Scoped, opts =>
{
    opts.ImplementationBase = typeof(BaseRepository);
});
```

### Adding Without Interface
You may want to use directly objects sometimes, if an abstraction has multiple implmentations. Then you can use `AddWithoutInterfaceToo` options as **true**.
If you set this option as true, object will be added into services without interface too, like following:

```csharp
services.AddServicesFrom("ProjectNamespace.Services", ServiceLifetime.Scoped, opts =>
{
    opts.AddWithoutInterfaceToo = true;
});
```

This makes something like following:

```csharp
services.AddScoped<IBookService, BookService>();
services.AddScoped<BookService>(); // < -- Adds without interface too.
services.AddScoped<IAuthorService, AuthorService>();
services.AddScoped<AuthorService>(); // <-- Also for each pattern matched objects.
//...
```

*** 

## Managing from Objects

You can manage your injections for class by class.

### Service Lifetime Attribute

```csharp
[ServiceLifeTime(ServiceLifetime.Singleton)] // <-- Only this object will be Singleton.
public class MyRepository : IMyRepository
{
    // ...
}
```

### Ignore Injection Attribute
You can ignore some of your class from injector.

```csharp
[IgnoreInjection] // <-- This object will not be added to services
public class MyRepository : IMyRepository
{
    // ...
}
```

### Register As Attribute
You can manage your service types to add into. This attribute can be used multiple time at once.

```csharp
/* 
 * Following object will be added into given types.
 */
[RegisterAs(typeof(IBookRepository))]
[RegisterAs(typeof(IBaseRepository<Book>), ServiceLifetime.Scoped)]
[RegisterAs(typeof(BookRepository), ServiceLifetime.Singleton)]
public class BookRepository : IBookRepository
{
    // ...
}
```
This injection will do following code:

```csharp
services.AddTransient<IBookRepository, BookRepository>();
services.AddScoped<IBaseRepository<Book>>, BookRepository>();
services.AddSingleton<BookRepository>();
```
