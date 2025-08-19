using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// API Validator for LibraryCreateDto.
    /// Ensures required fields for a new Library.
    /// </summary>
    public class LibraryCreateDtoApiValidator : AbstractValidator<LibraryCreateDto>
    {
        public LibraryCreateDtoApiValidator()
        {
            RuleFor(l => l.Name)
                .NotEmpty().WithMessage("Library name is required.");

            RuleFor(l => l.Address)
                .NotEmpty().WithMessage("Library address is required.");
        }
    }
}
