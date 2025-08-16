using FluentValidation;
using LibraryManagement.Application.DTOs;
using System;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// Validation rules for LoanCreateDto.
    /// Ensures that required fields are selected and dates are valid.
    /// </summary>
    public class LoanCreateDtoValidator : AbstractValidator<LoanCreateDto>
    {
        public LoanCreateDtoValidator()
        {
            // Validate that a library is selected (ID must be greater than 0)
            RuleFor(l => l.LibraryId)
                .GreaterThan(0)
                .WithMessage("Please select a library.");

            // Validate that a book is selected (ID must be greater than 0)
            RuleFor(l => l.BookId)
                .GreaterThan(0)
                .WithMessage("Please select a book.");

            // Validate that a member is selected (ID must be greater than 0)
            RuleFor(l => l.MemberId)
                .GreaterThan(0)
                .WithMessage("Please select a member.");

            // Due date must be greater than today
            RuleFor(l => l.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Due date must be after today.");
        }
    }
}
