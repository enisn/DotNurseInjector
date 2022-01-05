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
<a href="https://www.nuget.org/packages/DotNurse.Injector/">
 <img alt="Nuget" src="https://img.shields.io/nuget/v/DotNurse.Injector?logo=nuget&style=flat-square"></a>
<a href="https://www.codefactor.io/repository/github/enisn/dotnurseinjector">
 <img src="https://www.codefactor.io/repository/github/enisn/dotnurseinjector/badge" alt="CodeFactor" /></a>
<a href="https://gitmoji.carloscuesta.me">
  <img src="https://img.shields.io/badge/gitmoji-%20😜%20😍-FFDD67.svg?style=flat-square" alt="Gitmoji"></a>

---

## Getting Started

- Install Nuget package [from here](https://www.nuget.org/packages/DotNurse.Injector.AspNetCore/).

- Go to your **Startup.cs**, remove all your manual injections and use `AddServicesFrom()` method with namespace.

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

- ✅ That's it! DotNurse will scan your entire assembly and referenced assemblies to find types with the given namespace then register them to ServiceCollection.

### Managing in Objects
You can even define lifetimes and expose types from objects via using [[RegisterAs]](#register-as-attribute) attribute.

- Firstly, you should add following method for registering by attributes.
  ```csharp
  services.AddServicesByAttributes();
  ```
- Then you're ready to decorate your objects with `[RegisterAs]` attribute.
  ```csharp
  [RegisterAs(typeof(IBookRepository))]
  public class BookRepository : IBookRepository, IAnotherInterface, IYetAnothetInterface
  {
  }
  ```

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
[InjectService] public IBookRepository BookRepository { get; private set; }
```

```csharp
[InjectService] protected IBookRepository bookRepository;
```

#### Refactor your old codes
You can remove long constructor and place attribute to your fields or properties.

- Old code before DotNurseInjector
```csharp
public class BookService 
{
  public IBookRepository BookRepository { get; private set; }
  private readonly BookManager _bookManager;
  // ...
  
  public BookService(
    IBookRepository bookRepository,
    BookManager bookManager,
    // ... 
    )
  {
     BookRepository = bookRepository;
     _bookManager = bookManager;
     // ...
  }
}
```

- New code after DotNutseInjector
```csharp
public class BookService 
{
  [InjectService] private IBookRepository BookRepository { get; private set; }
  [InjectService] private readonly BookManager _bookManager;
  // ...
}
```

***

<img src="https://raw.githubusercontent.com/enisn/DotNurseInjector/master/art/dotnurse-github.png" alt="dotnurse-injector-social-preview" />

***

# Customizations

DotNurse meets your custom requirements such as defining lifetime, injecting into different interfaces, etc.

***

## Managing from Startup
.Nurse provides fluent API to manage your injections from a single point.

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

### Custom Registration Rule
You can use lambda expression to define your custom rules for registration. 

In example, you can use it for registering types in a namespace recursively.
```csharp
// Following code will register all types under Services namespace and sub-namespaces too.
services.AddServicesFrom(
    x => x.Namespace != null && (x.Namespace.StartsWith("MyProject.Services"));
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

### Ignoring Registration by Attribute
You can ignore some of your class from injector.

```csharp
[DontRegister] // <-- This object will not be added into services
public class MyRepository : IMyRepository
{
    // ...
}
```

### Register As Attribute
You can manage your service types to add into. This attribute can be used multiple times at once.

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
