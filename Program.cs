using HotChocolate.AspNetCore;
using HotChocolate.Demo.Application;
using HotChocolate.Demo.Data;
using HotChocolate.Demo.GraphQL;
using HotChocolate.Demo.GraphQL.DataLoaders;
using HotChocolate.Demo.GraphQL.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1) Persistence (EF Core, hier: InMemory fuer die Demo)
// -----------------------------------------------------------------------------
builder.Services.AddPooledDbContextFactory<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("library-demo"));

// -----------------------------------------------------------------------------
// 2) Application Services (Use-Cases)
// -----------------------------------------------------------------------------
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AuthorService>();

// -----------------------------------------------------------------------------
// 3) Authentication (Demo: symmetrischer JWT-Key)
// -----------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? "ENTWICKLUNGS-KEY-NUR-FUER-DEMO-ZWECKE-MIN-32-ZEICHEN";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Policy fuer Lese-Operationen
    options.AddPolicy("reader", p => p.RequireAuthenticatedUser());

    // Policy fuer schreibende Operationen (Rollen-basiert)
    options.AddPolicy("librarian", p =>
        p.RequireAuthenticatedUser().RequireRole("librarian", "admin"));
});

// -----------------------------------------------------------------------------
// 4) GraphQL Server Konfiguration
// -----------------------------------------------------------------------------
builder.Services
    .AddGraphQLServer()
    // Root-Types
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()

    // Object Types (explicit, Code-First)
    .AddType<BookType>()
    .AddType<AuthorType>()

    // Error-Filter (Mapping von Exceptions zu GraphQL-Errors)
    .AddErrorFilter<DomainErrorFilter>()

    // Middleware
    .AddFiltering()
    .AddSorting()
    .AddProjections()

    // Authorization
    .AddAuthorization()

    // Subscriptions (InMemory)
    .AddInMemorySubscriptions()

    // Persisted Queries (optional, fuer Production empfohlen)
    // .AddReadOnlyFileSystemQueryStorage("./persisted_queries")
    // .UsePersistedQueryPipeline()

    // Komplexitaets-/Sicherheits-Limits
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    })
    .AddMaxExecutionDepthRule(10)

    // DataLoader registrieren (Batch-Loader gegen N+1)
    .AddDataLoader<AuthorByIdDataLoader>()
    .AddDataLoader<BooksByAuthorIdDataLoader>();

// -----------------------------------------------------------------------------
// 5) App bauen
// -----------------------------------------------------------------------------
var app = builder.Build();

// Seed-Daten fuer die Demo
await DatabaseSeeder.SeedAsync(app.Services);

app.UseAuthentication();
app.UseAuthorization();

// WebSockets fuer Subscriptions
app.UseWebSockets();

// GraphQL-Endpunkt (inkl. Banana Cake Pop IDE unter /graphql)
app.MapGraphQL("/graphql");

app.Run();
