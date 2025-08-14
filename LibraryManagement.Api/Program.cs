using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configure services
// ----------------------

// Configurar DbContext para SQL Server (autenticación Windows)
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer("Server=.;Database=LibraryDB;Trusted_Connection=True;"));

// Registrar AutoMapper correctamente para .NET 8
// Opción: registrar todos los perfiles de la capa Application
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

// Registrar servicios de aplicación (CRUD completo)
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

// Añadir soporte para Razor Pages y API Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI solo en desarrollo
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------
// Configure middleware
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
