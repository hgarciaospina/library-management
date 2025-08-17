using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Repositories;
using FluentValidation;
using LibraryManagement.Application.Validations;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

/// ================================================
/// Database Context Configuration
/// ================================================
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(
        "Server=.;Database=LibraryDB;Trusted_Connection=True;Encrypt=False"
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
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();
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
        c.RoutePrefix = "swagger";
    });
}

// Quitamos HTTPS, solo HTTP local
// app.UseHttpsRedirection();  <- comentado o eliminado

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
