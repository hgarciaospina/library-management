using FluentValidation;
using LibraryManagement.Core.Entities;
using System;

namespace LibraryManagement.Application.Validations
{
    public class LoanValidator : AbstractValidator<Loan>
    {
        public LoanValidator()
        {
            // Validate that a library is selected
            RuleFor(l => l.LibraryId)
                .GreaterThan(0).WithMessage("Library must be selected.");

            // Validate that a book is selected
            RuleFor(l => l.BookId)
                .GreaterThan(0).WithMessage("Book must be selected.");

            // Validate that a member is selected
            RuleFor(l => l.MemberId)
                .GreaterThan(0).WithMessage("Member must be selected.");

            // Loan date must not be in the future
            RuleFor(l => l.LoanDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Loan date cannot be in the future.");

            // Due date must be after loan date
            RuleFor(l => l.DueDate)
                .GreaterThan(l => l.LoanDate).WithMessage("Due date must be after loan date.");

            // Return date (if set) must be after loan date
            RuleFor(l => l.ReturnDate)
                .GreaterThan(l => l.LoanDate)
                .When(l => l.ReturnDate.HasValue)
                .WithMessage("Return date must be after loan date.");
        }
    }
}
