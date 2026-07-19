# HotChocolate.Demo

Vollstaendiges Demo-Projekt fuer HotChocolate v14 mit .NET 8, das alle
wichtigen Konzepte aus der Dokumentation in lauffaehigem Code zeigt.

## Aufbau

```
HotChocolate.Demo/
├─ Domain/                   ← reine Modelle, keine Framework-Abhaengigkeiten
│  ├─ Book.cs
│  ├─ Author.cs
│  └─ DomainExceptions.cs
├─ Data/                     ← EF Core Persistence
│  ├─ LibraryDbContext.cs
│  └─ DatabaseSeeder.cs
├─ Application/              ← Use-Cases / Business-Logik
│  ├─ BookService.cs
│  └─ AuthorService.cs
├─ GraphQL/                  ← GraphQL-spezifisch, kennt Domain + Application
│  ├─ Query.cs
│  ├─ Mutation.cs
│  ├─ Subscription.cs
│  ├─ DomainErrorFilter.cs
│  ├─ Types/
│  │  ├─ BookType.cs
│  │  └─ AuthorType.cs
│  ├─ Inputs/
│  │  └─ BookInputs.cs
│  ├─ Payloads/
│  │  └─ BookPayloads.cs
│  └─ DataLoaders/
│     └─ AuthorByIdDataLoader.cs
├─ Program.cs                ← DI, Middleware, GraphQL-Server-Setup
├─ appsettings.json
└─ Examples.graphql          ← Beispiel-Queries fuer Banana Cake Pop
```

## Lokal starten

```bash
dotnet restore
dotnet run
```

Anschliessend im Browser oeffnen:

- Banana Cake Pop IDE: <http://localhost:5071/graphql>
- GraphQL Endpoint: POST <http://localhost:5071/graphql>

## Gezeigte Konzepte

| Konzept                               | Datei(en)                                    |
|---------------------------------------|----------------------------------------------|
| Code-First Schema                     | `Types/*.cs`                                 |
| ObjectType mit expliziter Nullability | `BookType.cs`, `AuthorType.cs`               |
| Queries + Paging + Filter + Sort      | `Query.cs` (`[UsePaging]`, `[UseFiltering]`) |
| Projections (SELECT-Optimierung)      | `Query.cs` (`[UseProjection]`)               |
| Mutations mit Input + Payload         | `Mutation.cs` + `Inputs/`, `Payloads/`       |
| Subscriptions via InMemory-Transport  | `Subscription.cs`, `Program.cs`              |
| DataLoader (Batch gegen N+1)          | `DataLoaders/AuthorByIdDataLoader.cs`        |
| GroupedDataLoader                     | `BooksByAuthorIdDataLoader`                  |
| Authentication (JWT)                  | `Program.cs`                                 |
| Authorization (Policies)              | `[Authorize(Policy = "...")]`                |
| Error-Mapping                         | `GraphQL/DomainErrorFilter.cs`               |
| Layered Architecture                  | `Domain` → `Application` → `GraphQL`         |
| EF Core via DbContextFactory          | `Program.cs`, Services                       |
| Query Depth Limit                     | `Program.cs` (`.AddMaxExecutionDepthRule`)   |

## Persisted Queries (Production)

Im `Program.cs` auskommentiert - fuer Production einschalten:

```csharp
.AddReadOnlyFileSystemQueryStorage("./persisted_queries")
.UsePersistedQueryPipeline()
```

Sicher gegen Query-Abuse, liebevoll zu deiner Infrastruktur.

## Production-Hinweise

- `IncludeExceptionDetails` nur in Development
- `MaxExecutionDepthRule` an die fachlichen Anforderungen anpassen
- `AllowIntrospection(false)` fuer nicht-oeffentliche APIs setzen
- JWT-Key **niemals** hardcoded - per `IConfiguration` + Secret Manager
- Subscriptions in Production: Redis oder Postgres statt InMemory
- DataLoader bei allen 1:N Navigationen - nie Lazy Loading in GraphQL!
