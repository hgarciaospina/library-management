using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Application.Validations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.Middleware;
using LibraryManagement.Application.Validators;
using LibraryManagement.Infrastructure.Seeding;
using System.Reflection;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

/// ================================================
/// DATABASE CONTEXT CONFIGURATION
/// ================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString += ";Encrypt=False;TrustServerCertificate=True";

builder.Services.AddDbContext<LibraryContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure());
});

/// ================================================
/// AUTOMAPPER CONFIGURATION
/// ================================================
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

/// ================================================
/// GENERIC REPOSITORY REGISTRATION
/// ================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

/// ================================================
/// APPLICATION SERVICES REGISTRATION
/// ================================================
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

/// ================================================
/// WEB VALIDATORS REGISTRATION (SYNC)
/// ================================================
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.MemberCreateDto>, MemberCreateDtoValidator>();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.BookCreateDto>, BookValidator>();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.LoanCreateDto>, LoanCreateDtoValidator>();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.LoanUpdateDto>, LoanUpdateDtoValidator>();

builder.Services.AddRazorPages();

/// ================================================
/// API BEHAVIOR CONFIGURATION
/// ================================================
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = errors,
            TraceId = context.HttpContext.TraceIdentifier
        });
    };
});

/// ================================================
/// SWAGGER / OPENAPI CONFIGURATION - FIXED
/// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management API",
        Version = "1.0",
        Description = "API for managing library operations including books, members, loans, and libraries.",
        Contact = new OpenApiContact
        {
            Name = "Library Management Team",
            Email = "support@librarymanagement.com"
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add examples for all DTOs
    c.ExampleFilters();

    // Add detailed schema descriptions
    c.SchemaFilter<LibraryManagementSchemaFilter>();

    // CRITICAL: Force discovery of all controllers and actions
    c.DocInclusionPredicate((docName, apiDesc) => true);
});

// Register all Swagger Examples for Create/Update DTOs
builder.Services.AddSwaggerExamplesFromAssemblyOf<MemberCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BookCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LibraryCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoanCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<MemberUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BookUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LibraryUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoanUpdateExample>();

var app = builder.Build();

/// ================================================
/// DATABASE MIGRATION & SEEDING
/// ================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LibraryContext>();
        DatabaseInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

/// ================================================
/// MIDDLEWARE PIPELINE
/// ================================================
app.UseExceptionHandlingMiddleware();

// Configure Swagger UI
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1.0");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Library Management API Documentation";
    c.DefaultModelsExpandDepth(-1);
    c.DisplayOperationId();
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

/// ================================================
/// ENDPOINT MAPPING - CRITICAL FOR API DISCOVERY
/// ================================================
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

/// ================================================
/// SWAGGER EXAMPLES FOR CREATE DTOS
/// ================================================
#region CREATE EXAMPLES

public class MemberCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.MemberCreateDto>
{
    public LibraryManagement.Application.DTOs.MemberCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.MemberCreateDto
        {
            FirstName = "Henry",
            LastName = "García Ospina",
            Email = "henrygarciaospina@gmail.com",
            PhoneNumber = "+573001234567",
            LibraryId = 1
        };
}

public class BookCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.BookCreateDto>
{
    public LibraryManagement.Application.DTOs.BookCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.BookCreateDto
        {
            Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
            Author = "Robert C. Martin",
            ISBN = "978-0132350884",
            PublicationYear = 2008,
            LibraryId = 1,
            IsAvailable = true
        };
}

public class LoanCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LoanCreateDto>
{
    public LibraryManagement.Application.DTOs.LoanCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LoanCreateDto
        {
            LibraryId = 1,
            BookId = 1,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(14)
        };
}

public class LibraryCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LibraryCreateDto>
{
    public LibraryManagement.Application.DTOs.LibraryCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LibraryCreateDto
        {
            Name = "Central Public Library",
            Address = "123 Main Street, Downtown, Cityville"
        };
}

#endregion

/// ================================================
/// SWAGGER EXAMPLES FOR UPDATE DTOS
/// ================================================
#region UPDATE EXAMPLES

public class MemberUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.MemberUpdateDto>
{
    public LibraryManagement.Application.DTOs.MemberUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.MemberUpdateDto
        {
            Id = 1,
            FirstName = "Henry Updated",
            LastName = "García Ospina",
            Email = "henry.updated@example.com",
            PhoneNumber = "+573009876543",
            LibraryId = 1
        };
}

public class BookUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.BookUpdateDto>
{
    public LibraryManagement.Application.DTOs.BookUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.BookUpdateDto
        {
            Id = 1,
            Title = "Clean Code: Updated Edition",
            Author = "Robert C. Martin",
            ISBN = "978-0132350884",
            PublicationYear = 2008,
            LibraryId = 1,
            IsAvailable = false
        };
}

public class LibraryUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LibraryUpdateDto>
{
    public LibraryManagement.Application.DTOs.LibraryUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LibraryUpdateDto
        {
            Id = 1,
            Name = "Central Public Library - Renovated",
            Address = "456 Oak Avenue, Uptown, Cityville"
        };
}

public class LoanUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LoanUpdateDto>
{
    public LibraryManagement.Application.DTOs.LoanUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LoanUpdateDto
        {
            Id = 1,
            LibraryId = 1,
            BookId = 1,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(21),
            ReturnDate = DateTime.UtcNow.AddDays(14),
            LoanDate = DateTime.UtcNow.AddDays(-7)
        };
}

#endregion

/// ================================================
/// CUSTOM SCHEMA FILTER FOR BETTER DOCUMENTATION
/// ================================================
public class LibraryManagementSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(LibraryManagement.Application.DTOs.BookCreateDto))
        {
            schema.Description = "Data transfer object for creating a new book in the library system.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.BookDto))
        {
            schema.Description = "Data transfer object representing a book with all details including library information.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.LibraryDto))
        {
            schema.Description = "Data transfer object representing a library location.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.LibraryUpdateDto))
        {
            schema.Description = "Data transfer object for updating an existing library.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.LoanCreateDto))
        {
            schema.Description = "Data transfer object for creating a new book loan.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.LoanDetailsDto))
        {
            schema.Description = "Data transfer object containing complete details of a book loan.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.LoanDto))
        {
            schema.Description = "Data transfer object representing a book loan with related information.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.LoanUpdateDto))
        {
            schema.Description = "Data transfer object for updating an existing book loan, primarily for recording returns.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.MemberCreateDto))
        {
            schema.Description = "Data transfer object for creating a new library member.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.MemberDto))
        {
            schema.Description = "Data transfer object representing a library member with all details.";
        }
        else if (context.Type == typeof(LibraryManagement.Application.DTOs.MemberUpdateDto))
        {
            schema.Description = "Data transfer object for updating an existing library member's information.";
        }
    }
}