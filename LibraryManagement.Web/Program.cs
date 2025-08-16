using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Repositories;
using FluentValidation;
using LibraryManagement.Application.Validations; // Validadores de todas las entidades
using FluentValidation.AspNetCore; // Necesario para activar la integración con ASP.NET Core

var builder = WebApplication.CreateBuilder(args);

// ================================================
// Configurar DbContext
// ================================================
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(
        "Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
    )
);

// ================================================
// AutoMapper
// ================================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// ================================================
// Repositorio genérico
// ================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ================================================
// Servicios de aplicación
// ================================================
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

// ================================================
// Registrar validadores FluentValidation
// ================================================
// Detecta automáticamente todos los validadores dentro del ensamblado de Validations
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();

// Activa la integración de FluentValidation con ASP.NET Core para que las reglas se apliquen automáticamente
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// ================================================
// Razor Pages y Controllers
// ================================================
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// ================================================
// Swagger/OpenAPI
// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ================================================
// Pipeline
// ================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
