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

var builder = WebApplication.CreateBuilder(args);

/// ================================================
/// Database Context Configuration
/// ================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Ajuste para desarrollo local: deshabilita cifrado SSL y confía en certificados autofirmados
connectionString += ";Encrypt=False;TrustServerCertificate=True";

builder.Services.AddDbContext<LibraryContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure() // Manejo de errores transitorios
    );
});

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
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        // Registra automáticamente todos los validators de la capa Application
        fv.RegisterValidatorsFromAssemblyContaining<BookValidator>();
        fv.AutomaticValidationEnabled = true; // <-- clave para que valide automáticamente
    });

builder.Services.AddRazorPages();

/// ================================================
/// Swagger/OpenAPI
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
});

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

// Solo HTTP
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

/// ================================================
/// Endpoint Mapping
/// ================================================
app.MapControllers(); // Los controllers con [ApiController] tomarán las validaciones automáticamente
app.MapRazorPages();

app.Run();
