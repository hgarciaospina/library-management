using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validations
{
    public class MemberUpdateDtoValidator : AbstractValidator<MemberUpdateDto>
    {
        public MemberUpdateDtoValidator()
        {
            RuleFor(m => m.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(m => m.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(m => m.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Invalid phone number format.");

            RuleFor(m => m.LibraryId)
                .GreaterThan(0).WithMessage("Library must be selected.");
        }
    }
}
