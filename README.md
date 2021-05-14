# dotnet-mockup-be

This is a mock-up project, that will help you to create project from scratch. You will need just changed DB provider (by default it uses PostgreSQL) and some configurations like: JWT token lifetime, email credentials etc. The project implements Multilayer architecture:
- **DummyWebApp** - Presentation layer
- **DummyWebApp.BLL** - Application layer
- **DummyWebApp.DAL**  - Data layer
- **DummyWebApp.Core** - contains common information
#### Notes
This project uses `StyleCop.Analyzer` library that will help you stick to C# code convention, but **Resharper** should be enabled in your IDE. All rules is located in `/Solution.ruleset` ruleset file.
More information about this library you can find [here](https://github.com/DotNetAnalyzers/StyleCopAnalyzers "here")
### DummyWebApp
This is API (.NET 5 WEB API) project that already contains Swagger documentation with XML supporting, JWT based auth, error filter. Also this project has implemented localization with **en** and **uk** languages. All localization files are in /DummyWebApp/Resolurces folder, localization also supports anotation. This project is the higher layer in Multitier architecture.

#### DummyWebApp.BLL
This project represents Application Layer, that controls an application’s functionality by performing detailed processing. It contains busines services, DTOs, validation attributes, ect. Each services returns IResult or IResult<T> interface, that mean that anyone service will never throw exception and result information is wrapped in result model. It is very useful in Presentation layer (API project) for error handling and logging. 
##### `IResult` implementation
```csharp
    public interface IResult
    {
        IReadOnlyCollection<string> Messages { get; }

        bool Success { get; }

        Exception? Exception { get; }
    }
```

##### `IResult<T>` implementation
```csharp
    public interface IResult<out TData> : IResult
    {
        TData Data { get; }
    }
```

##### Implementation of `ErrorableResultFilterAttribute` that uses `IResult` and `IResult<T>`

```csharp
public class ErrorableResultFilterAttribute : ResultFilterAttribute
    {
        private const string Errors = nameof(Errors);

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ErrorableActionResult actionResult)
            {
                if (!actionResult.Result.Success)
                {
                    var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<ErrorableResultFilterAttribute>>();
                    var error = new SerializableError
                    {
                        { Errors, actionResult.Result.Messages.Select(m => localizer[m].Value) }
                    };

                    context.Result = new BadRequestObjectResult(error);
                    LogFailureResult(actionResult.Result, context
                        .HttpContext
                        .RequestServices
                        .GetRequiredService<ILogger<ErrorableResultFilterAttribute>>());

                    return;
                }

                if (context.HttpContext.Request.Method == "DELETE")
                {
                    context.Result = new NoContentResult();

                    return;
                }

                if (actionResult.Result is IResult<object> objectResult)
                    context.Result = new OkObjectResult(objectResult.Data);
                else
                    context.Result = new OkResult();
            }
        }
```

#### DummyWebApp.DAL
This Data Access Layer in Multitier architecture. It includes the data persistence mechanisms (database servers, file shares, etc.) and the data access layer that encapsulates the persistence mechanisms and exposes the data. It uses EF Core and **ApplicationDbContext** as contex of DB. Also this project contains DbSeeder class that is responsible for DB seeding and Entities folder where are DB entities are located.

#### DummyWebApp.Tests
This **xUnit** porject that contains unit tests. It already has few unit tests implementaion: `AuthServiceTests` and `ResetPasswordTokenProviderTests`.

#### Environment variables
In addition to `appsettings.json` configuration file, project uses `.env` files for configuration. For example `config.Development.env`. This file contains environment variables. If you want to use another environment, you will need just create another .env file with name of your environment.
##### Environment file schema
*{config}.{EnvironmentName}.env*

*{EnvironmentName}* - name of app environment (Production, Development).

*{config}* - constant name
##### Environment variables
- **CONNECTION_STRING** - connection string for DB.
