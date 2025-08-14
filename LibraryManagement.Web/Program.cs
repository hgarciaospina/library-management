
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext para SQL Server (autenticación Windows)
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(
        "Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
    )
);

/// Registrar AutoMapper (solo pasando el tipo del perfil)
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});


//Seregistra el repositorio génerico
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Registrar servicios de aplicación
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

// Razor Pages y API Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
