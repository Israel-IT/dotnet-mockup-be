# dotnet-mockup-be

### DummyWebApp
This is API (.NET 5 WEB API) project that already contains Swagger documentation with XML supporting, JWT based auth, error filter. Also this project has implemented localization with **en** and **uk** languages. All localization files are in /DummyWebApp/Resolurces folder, localization also supports anotation. This project is the higher layer in Multitier architecture.

#### DummyWebApp.BLL
This project represents Application Layer, that controls an application’s functionality by performing detailed processing. It contains busines services, DTOs, validation attributes, ect. Each services returns IResult or IResult<T> interface, that mean that anyone service will never throw exception and result information is wrapped in result model. It is very useful in Presentation layer (API project) for error handling and logging. 
`IResult` implementation
```csharp
    public interface IResult
    {
        IReadOnlyCollection<string> Messages { get; }

        bool Success { get; }

        Exception? Exception { get; }
    }
```

`IResult<T> ` implementation
```csharp
    public interface IResult<out TData> : IResult
    {
        TData Data { get; }
    }
```

#### DummyWebApp.DAL
This Data Access Layer in Multitier architecture. It includes the data persistence mechanisms (database servers, file shares, etc.) and the data access layer that encapsulates the persistence mechanisms and exposes the data. It uses EF Core and **ApplicationDbContext** as contex of DB. Also this project contains DbSeeder class that is responsible for DB seeding and Entities folder where are DB entities are located.

#### DummyWebApp.Tests
This **xUnit** porject that contains unit tests. It already has few unit tests implementaion: `AuthServiceTests` and `ResetPasswordTokenProviderTests`.
