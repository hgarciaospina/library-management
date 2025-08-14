using FluentValidation;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Application.Validations
{
    public class LibraryValidator : AbstractValidator<Library>
    {
        public LibraryValidator()
        {
            RuleFor(l => l.Name)
                .NotEmpty().WithMessage("Library name is required.")
                .MaximumLength(100).WithMessage("Library name cannot exceed 100 characters.");

            RuleFor(l => l.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
        }
    }
}
