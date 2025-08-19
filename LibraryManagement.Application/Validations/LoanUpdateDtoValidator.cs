using FluentValidation;
using LibraryManagement.Application.DTOs;
using System;

namespace LibraryManagement.Application.Validators
{
    /// <summary>
    /// Validator for updating a Loan.
    /// Ensures:
    /// - BookId is selected.
    /// - DueDate is required and cannot be before LoanDate.
    /// - ReturnDate, if provided, cannot be earlier than LoanDate and cannot be after today.
    /// - All date comparisons ignore time component.
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
            // DueDate is required and must be >= LoanDate (ignoring time)
            // ==============================
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.")
                .Must((dto, dueDate) => dueDate.Date >= dto.LoanDate.Date)
                .WithMessage("Due date cannot be before the loan date.");

            // ==============================
            // ReturnDate, if provided, must be >= LoanDate (ignoring time) and <= today
            // ==============================
            RuleFor(x => x.ReturnDate)
                .Must((dto, returnDate) =>
                    !returnDate.HasValue || returnDate.Value.Date >= dto.LoanDate.Date)
                .WithMessage("Return date cannot be earlier than the loan date.")
                .Must(returnDate =>
                    !returnDate.HasValue || returnDate.Value.Date <= DateTime.Today)
                .WithMessage("Return date cannot be in the future.");

            // ==============================
            // Additional date rules can be added here if needed
            // Example: RenewalDate, etc.
            // ==============================
        }
    }
}
