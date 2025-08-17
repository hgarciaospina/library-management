using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validators
{
    /// <summary>
    /// Validator for updating a Loan.
    /// Ensures BookId and DueDate are valid.
    /// ReturnDate, if provided, cannot be earlier than LoanDate (loan creation date).
    /// </summary>
    public class LoanUpdateDtoValidator : AbstractValidator<LoanUpdateDto>
    {
        public LoanUpdateDtoValidator()
        {
            // Book must be selected
            RuleFor(x => x.BookId)
                .GreaterThan(0)
                .WithMessage("Book must be selected.");

            // DueDate is required
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.");

            // ReturnDate cannot be earlier than LoanDate
            RuleFor(x => x.ReturnDate)
                .GreaterThanOrEqualTo(x => x.LoanDate)
                .When(x => x.ReturnDate.HasValue)
                .WithMessage("Return date cannot be earlier than the loan date.");
        }
    }
}
