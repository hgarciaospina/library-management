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
using LibraryManagement.Application.Validations.Api; // ✅ Custom exception handling middleware

var builder = WebApplication.CreateBuilder(args);

/// ================================================
/// DATABASE CONTEXT CONFIGURATION
/// ================================================
/// Configures SQL Server connection for EF Core
/// Includes retry policy for transient failures
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString += ";Encrypt=False;TrustServerCertificate=True"; // Dev-only adjustment

builder.Services.AddDbContext<LibraryContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure());
});

/// ================================================
/// AUTOMAPPER CONFIGURATION
/// ================================================
/// Registers AutoMapper profile for mapping Entities <-> DTOs
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

/// ================================================
/// GENERIC REPOSITORY REGISTRATION
/// ================================================
/// Adds generic repository pattern for all entities
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

/// ================================================
/// APPLICATION SERVICES REGISTRATION
/// ================================================
/// Registers business/application services (Dependency Injection)
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

/// ================================================
/// WEB VALIDATORS REGISTRATION (SYNC) - RAZOR PAGES AND MVC
/// ================================================
/// These validators are synchronous and safe for automatic ASP.NET validation.
/// They will run automatically when using Razor Pages or MVC forms.
/// Async validators like MustAsync MUST NOT be registered here.
/// 


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.MemberCreateDto>, MemberCreateDtoValidator>();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.BookCreateDto>, BookValidator>();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.LoanCreateDto>, LoanCreateDtoValidator>();
builder.Services.AddScoped<IValidator<LibraryManagement.Application.DTOs.LoanUpdateDto>, LoanUpdateDtoValidator>();

builder.Services.AddScoped<MemberCreateDtoApiValidator>();

/// ✅ Register RazorPages (uses sync validators automatically)
builder.Services.AddRazorPages();

/// ================================================
/// API VALIDATORS REGISTRATION (ASYNC) - MANUAL INVOCATION
/// ================================================
/// These validators contain asynchronous rules (e.g., MustAsync) and should NOT
/// be registered for automatic ASP.NET validation.
/// They should only be injected manually inside your application services.
builder.Services.AddScoped<MemberCreateDtoApiValidator>();
builder.Services.AddScoped<BookCreateDtoApiValidator>();
builder.Services.AddScoped<LoanCreateDtoApiValidator>();
builder.Services.AddScoped<LoanUpdateDtoApiValidator>();

/// ================================================
/// API BEHAVIOR CONFIGURATION
/// ================================================
/// Customizes error response when ModelState is invalid.
/// Ensures clients receive structured validation errors.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
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
            Errors = errors
        });
    };
});

/// ================================================
/// SWAGGER / OPENAPI CONFIGURATION
/// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "Library Management System API endpoints. " +
                      "Includes CRUD operations for Books, Members, Loans, and Libraries. " +
                      "Use Swagger UI to test requests with preloaded examples."
    });
    c.ExampleFilters(); // Enables example request/response
});

// Register example providers for all Create DTOs
builder.Services.AddSwaggerExamplesFromAssemblyOf<MemberCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BookCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LibraryCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoanCreateExample>();

// Register example providers for all Update DTOs
builder.Services.AddSwaggerExamplesFromAssemblyOf<MemberUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BookUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LibraryUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoanUpdateExample>();

var app = builder.Build();

/// ================================================
/// MIDDLEWARE PIPELINE
/// ================================================

// ✅ Global exception handling middleware (catches validation + server errors)
app.UseExceptionHandlingMiddleware();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V1");
    c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
});

// Uncomment if HTTPS redirection is required
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

/// ================================================
/// ENDPOINT MAPPING
/// ================================================
app.MapControllers();
app.MapRazorPages();

app.Run();

/// ================================================
/// SWAGGER EXAMPLES FOR CREATE DTOS
/// ================================================
#region CREATE EXAMPLES

/// <summary>
/// Example of creating a Member
/// </summary>
public class MemberCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.MemberCreateDto>
{
    public LibraryManagement.Application.DTOs.MemberCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.MemberCreateDto
        {
            FirstName = "Henry",
            LastName = "García Ospina",
            Email = "henrygarciaospina@gmail.com",
            PhoneNumber = "+573001234567",
            LibraryId = 7
        };
}

/// <summary>
/// Example of creating a Book
/// </summary>
public class BookCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.BookCreateDto>
{
    public LibraryManagement.Application.DTOs.BookCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.BookCreateDto
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "978-0132350884",
            PublicationYear = 2008,
            LibraryId = 7,
            IsAvailable = true
        };
}

/// <summary>
/// Example of creating a Loan
/// </summary>
public class LoanCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LoanCreateDto>
{
    public LibraryManagement.Application.DTOs.LoanCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LoanCreateDto
        {
            LibraryId = 7,
            BookId = 1,
            MemberId = 4,
            DueDate = DateTime.UtcNow.AddDays(14)
        };
}

/// <summary>
/// Example of creating a Library
/// </summary>
public class LibraryCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LibraryCreateDto>
{
    public LibraryManagement.Application.DTOs.LibraryCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LibraryCreateDto
        {
            Name = "Central Library",
            Address = "123 Main St, City"
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
            Id = 3,
            FirstName = "Henry",
            LastName = "García Ospina",
            Email = "henrygarciaospina@gmail.com",
            PhoneNumber = "+573001234567",
            LibraryId = 7
        };
}

public class BookUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.BookUpdateDto>
{
    public LibraryManagement.Application.DTOs.BookUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.BookUpdateDto
        {
            Id = 15,
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "978-0132350884",
            PublicationYear = 2008,
            LibraryId = 7,
            IsAvailable = true
        };
}

public class LibraryUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LibraryUpdateDto>
{
    public LibraryManagement.Application.DTOs.LibraryUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LibraryUpdateDto
        {
            Id = 7,
            Name = "Central Library",
            Address = "123 Main St, City"
        };
}

public class LoanUpdateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LoanUpdateDto>
{
    public LibraryManagement.Application.DTOs.LoanUpdateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LoanUpdateDto
        {
            Id = 21,
            LibraryId = 7,
            BookId = 15,
            MemberId = 3,
            DueDate = DateTime.UtcNow.AddDays(14),
            ReturnDate = null
        };
}

#endregion

/// ================================================
/// CRUD ENDPOINT DOCUMENTATION FOR SWAGGER
/// ================================================

/*
 * BOOKS:
 * GET /api/books - List all books
 * GET /api/books/{id} - Get book by ID
 * POST /api/books - Create a book (BookCreateDto)
 * PUT /api/books/{id} - Update a book (BookUpdateDto)
 * DELETE /api/books/{id} - Delete a book
 * 
 * MEMBERS:
 * GET /api/members
 * GET /api/members/{id}
 * POST /api/members
 * PUT /api/members/{id}
 * DELETE /api/members/{id}
 *
 * LOANS:
 * GET /api/loans
 * GET /api/loans/{id}
 * POST /api/loans
 * PUT /api/loans/{id}
 * DELETE /api/loans/{id}
 *
 * LIBRARIES:
 * GET /api/libraries
 * GET /api/libraries/{id}
 * POST /api/libraries
 * PUT /api/libraries/{id}
 * DELETE /api/libraries/{id}
 *
 * VALIDATIONS:
 * - Emails must be in valid format
 * - ISBN must follow standard format
 * - Loan/Return dates: ReturnDate >= LoanDate
 * - All required fields cannot be null
 *
 * SWAGGER USAGE:
 * - Navigate to /swagger
 * - Select endpoint
 * - Click "Try it out"
 * - Example requests for Create/Update DTOs are preloaded
 */

