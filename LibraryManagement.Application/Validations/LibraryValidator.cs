using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validations
{
    public class LibraryValidator : AbstractValidator<LibraryCreateDto>
    {
        public LibraryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
        }
    }
}
