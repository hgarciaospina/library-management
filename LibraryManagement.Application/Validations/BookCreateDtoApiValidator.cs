using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// API Validator for BookCreateDto.
    /// Ensures required fields, valid ISBN, year constraints, 
    /// and referenced Library existence.
    /// </summary>
    public class BookCreateDtoApiValidator : AbstractValidator<BookCreateDto>
    {
        public BookCreateDtoApiValidator(ILibraryService libraryService)
        {
            // Title required
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            // Author required
            RuleFor(b => b.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(150).WithMessage("Author cannot exceed 150 characters.");

            // ISBN required + basic format check (ISBN-10 or ISBN-13)
            RuleFor(b => b.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Matches(@"^(97(8|9))?\d{9}(\d|X)$")
                .WithMessage("Invalid ISBN format. Use ISBN-10 or ISBN-13.");

            // PublicationYear between 1450 and current year
            RuleFor(b => b.PublicationYear)
                .InclusiveBetween(1450, DateTime.UtcNow.Year)
                .WithMessage($"Publication year must be between 1450 and {DateTime.UtcNow.Year}.");

            // LibraryId required and must exist
            RuleFor(b => b.LibraryId)
                .NotNull().WithMessage("LibraryId is required.")
                .Must(id => id > 0)
                .WithMessage("LibraryId must be greater than zero.")
                .MustAsync(async (id, cancellation) =>
                {
                    if (!id.HasValue) return false; // Defensive null check
                    var library = await libraryService.GetByIdAsync(id.Value);
                    return library != null;
                })
                .WithMessage("The selected library does not exist.");

            // IsAvailable no necesita validación porque viene por defecto en true/false
        }
    }
}
