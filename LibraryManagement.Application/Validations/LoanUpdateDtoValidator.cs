using FluentValidation;
using LibraryManagement.Application.DTOs;
using System;

namespace LibraryManagement.Application.Validators
{
    /// <summary>
    /// Validator for updating a Loan.
    /// Ensures BookId and DueDate are valid.
    /// ReturnDate, if provided, cannot be earlier than LoanDate and cannot be after today's date.
    /// </summary>
    public class LoanUpdateDtoValidator : AbstractValidator<LoanUpdateDto>
    {
        public LoanUpdateDtoValidator()
        {
            // ==============================
            // Book must be selected
            // ==============================
            RuleFor(x => x.BookId)
                .GreaterThan(0)
                .WithMessage("Book must be selected.");

            // ==============================
            // DueDate is required
            // ==============================
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.");

            // ==============================
            // ReturnDate cannot be earlier than LoanDate
            // and cannot be after the current date
            // ==============================
            RuleFor(x => x.ReturnDate)
                .GreaterThanOrEqualTo(x => x.LoanDate)
                .When(x => x.ReturnDate.HasValue)
                .WithMessage("Return date cannot be earlier than the loan date.");

            RuleFor(x => x.ReturnDate)
                .LessThanOrEqualTo(DateTime.Today)
                .When(x => x.ReturnDate.HasValue)
                .WithMessage("Return date cannot be in the future.");
        }
    }
}
