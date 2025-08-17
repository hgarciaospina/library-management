using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Repositories;
using FluentValidation;
using LibraryManagement.Application.Validations;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

/// ================================================
/// Database Context Configuration
/// ================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString += ";Encrypt=False;TrustServerCertificate=True"; // Dev adjustment

builder.Services.AddDbContext<LibraryContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure());
});

/// ================================================
/// AutoMapper Configuration
/// ================================================
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

/// ================================================
/// Generic Repository Registration
/// ================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

/// ================================================
/// Application Services Registration
/// ================================================
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

/// ================================================
/// FluentValidation Configuration
/// ================================================
// Register validators for all DTOs
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<MemberCreateDtoValidator>();
        fv.AutomaticValidationEnabled = true; // enable automatic validation
    });

builder.Services.AddRazorPages();

/// ================================================
/// ApiBehavior configuration for full validation errors
/// ================================================
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
/// Swagger/OpenAPI Configuration
/// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "API endpoints for Library Management System"
    });

    // Enable examples from Swashbuckle.AspNetCore.Filters
    c.ExampleFilters();
});

// Register example providers for Create DTOs
builder.Services.AddSwaggerExamplesFromAssemblyOf<MemberCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BookCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LibraryCreateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoanCreateExample>();

// Register example providers for Update DTOs
builder.Services.AddSwaggerExamplesFromAssemblyOf<MemberUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BookUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LibraryUpdateExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoanUpdateExample>();

var app = builder.Build();

/// ================================================
/// Middleware Pipeline
/// ================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V1");
    c.RoutePrefix = "swagger";
});

// HTTP only
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

/// ================================================
/// Endpoint Mapping
/// ================================================
app.MapControllers();
app.MapRazorPages();

app.Run();

/// ================================================
/// Swagger Example Classes for Create DTOs
/// ================================================
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

public class BookCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.BookCreateDto>
{
    public LibraryManagement.Application.DTOs.BookCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.BookCreateDto
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "9780132350884",
            PublicationYear = 2008,
            LibraryId = 7,
            IsAvailable = true
        };
}

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

public class LibraryCreateExample : IExamplesProvider<LibraryManagement.Application.DTOs.LibraryCreateDto>
{
    public LibraryManagement.Application.DTOs.LibraryCreateDto GetExamples() =>
        new LibraryManagement.Application.DTOs.LibraryCreateDto
        {
            Name = "Central Library",
            Address = "123 Main St, City"
        };
}

/// ================================================
/// Swagger Example Classes for Update DTOs
/// ================================================
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
            ISBN = "9780132350884",
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
