 # DotNurseInjector


<table border="0">
<tr>
<td width="20%"> <img width="75" src="https://raw.githubusercontent.com/enisn/DotNurseInjector/main/art/dotnurse-icon.png" /> </td>

<td width="80%">
<h2>.Nurse Injector</h2>
.Nurse is automatic dependency Injector for dotnet.
 </td>
</tr>
</table>


---

## Getting Started

- Install Nuget package

- Go your **Startup.cs**, remove all your manual injections and use `AddServicesFrom()` method with namespace.

```csharp

services.AddTransient<IBookRepository, BookRepository>();
services.AddTransient<IAuthorRepository, AuthorRepository>();
services.AddTransient<IPublisherRepository, PublisherRepository>();
//...

/* REPLACE THEM WITH FOLLOWING: */

services.AddServicesFrom("MyCompany.ProjectName.Repositories.Concrete"); // <-- Your implementations namespace.

```

- That's it! DotNurse can find your namespace from any assembly. You don't need to send any Assembly parameter.


***

***

## Customizations

DotNurse meets your custom requirements such as a defining lifetime, injecting into different interfaces etc.

***

### Managing from Startup

.Nurse provides fluent api to manage your injections from single point.

#### Service Lifetime

```csharp
services.AddServicesFrom("MyCompany.ProjectName.Services", ServiceLifetime.Scoped);
```