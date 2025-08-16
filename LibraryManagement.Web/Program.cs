using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Repositories;
using FluentValidation;
using LibraryManagement.Application.Validations; // Validators for all entities
using FluentValidation.AspNetCore; // Required to enable FluentValidation integration with ASP.NET Core

var builder = WebApplication.CreateBuilder(args);

/// ================================================
/// Database Context Configuration
/// ================================================
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(
        "Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
    )
);

/// ================================================
/// AutoMapper Configuration
/// ================================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

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
// Automatically detects and registers all validators from the Validations assembly
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();

// Enables FluentValidation integration with ASP.NET Core (server + client side)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

/// ================================================
/// MVC + Razor Pages
/// ================================================
builder.Services.AddRazorPages();
builder.Services.AddControllers();

/// ================================================
/// Swagger/OpenAPI
/// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/// ================================================
/// Middleware Pipeline
/// ================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V1");
        c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Maps for Controllers and Razor Pages
app.MapControllers();
app.MapRazorPages();

app.Run();
