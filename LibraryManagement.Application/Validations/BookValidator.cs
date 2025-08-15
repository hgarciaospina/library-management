using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// Validador para operaciones de creación y actualización de libros.
    /// Se aplica al DTO BookCreateDto para proteger la integridad de los datos recibidos desde la UI.
    /// </summary>
    public class BookValidator : AbstractValidator<BookCreateDto>
    {
        public BookValidator()
        {
            // Título del libro
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            // Autor del libro
            RuleFor(b => b.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author cannot exceed 100 characters.");

            // ISBN con formato específico: 000-0000000000
            RuleFor(b => b.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Matches(@"^\d{3}-\d{10}$").WithMessage("ISBN must be in the format 000-0000000000.");

            // Año de publicación entre 1500 y el año actual
            RuleFor(b => b.PublicationYear)
             .NotNull().WithMessage("Publication year is required.")
    .            InclusiveBetween(1500, DateTime.Now.Year)
    .            WithMessage($"Publication year must be between 1500 and {DateTime.Now.Year}.");

            // Librería seleccionada (no puede ser cero o negativo)
            RuleFor(b => b.LibraryId)
                .GreaterThan(0).WithMessage("Library must be selected.");
        }
    }
}
