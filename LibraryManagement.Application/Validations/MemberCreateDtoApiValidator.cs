using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;

namespace LibraryManagement.Application.Validations.Api
{
    /// <summary>
    /// API Validator for MemberCreateDto.
    /// Ensures required fields and validates referenced Library.
    /// </summary>
    public class MemberCreateDtoApiValidator : AbstractValidator<MemberCreateDto>
    {
        public MemberCreateDtoApiValidator(ILibraryService libraryService)
        {
            // FirstName is required
            RuleFor(m => m.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            // LastName is required
            RuleFor(m => m.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            // Email is required + format validation
            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.");

            // Phone is optional but if present, it must follow a valid format
            RuleFor(m => m.PhoneNumber)
                .Matches(@"^\+?\d{7,15}$")
                .When(m => !string.IsNullOrWhiteSpace(m.PhoneNumber))
                .WithMessage("Phone number format is invalid. Use international format, e.g., +123456789.");

            //// LibraryId is required and must exist
            //RuleFor(m => m.LibraryId)
            //    .NotNull().WithMessage("LibraryId is required.")
            //    .Must(id => id > 0).WithMessage("LibraryId must be greater than zero.")
            //    .MustAsync(async (id, cancellation) =>
            //    {
            //        if (!id.HasValue) return false; // Defensive check for null
            //        var library = await libraryService.GetByIdAsync(id.Value);
            //        return library != null;
            //    })
            //    .WithMessage("The selected library does not exist.");
        }
    }
}
