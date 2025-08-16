//using FluentValidation;
//using LibraryManagement.Core.Entities;
//using System;

//namespace LibraryManagement.Application.Validations
//{
//    /// <summary>
//    /// Validation rules for the Loan entity.
//    /// Ensures that required fields are selected and dates are consistent.
//    /// </summary>
//    public class LoanValidator : AbstractValidator<Loan>
//    {
//        public LoanValidator()
//        {
//            // Validate that a library is selected (ID must be greater than 0)
//            RuleFor(l => l.LibraryId)
//                .GreaterThan(0)
//                .WithMessage("Please select a library.");

//            // Validate that a book is selected (ID must be greater than 0)
//            RuleFor(l => l.BookId)
//                .GreaterThan(0)
//                .WithMessage("Please select a book.");

//            // Validate that a member is selected (ID must be greater than 0)
//            RuleFor(l => l.MemberId)
//                .GreaterThan(0)
//                .WithMessage("Please select a member.");

//            // Loan date must not be in the future
//            RuleFor(l => l.LoanDate)
//                .LessThanOrEqualTo(DateTime.UtcNow)
//                .WithMessage("Loan date cannot be in the future.");

//            // Due date must be strictly after loan date
//            RuleFor(l => l.DueDate)
//                .GreaterThan(l => l.LoanDate)
//                .WithMessage("Due date must be after loan date.");

//            // Return date (if set) must also be strictly after loan date
//            RuleFor(l => l.ReturnDate)
//                .GreaterThan(l => l.LoanDate)
//                .When(l => l.ReturnDate.HasValue)
//                .WithMessage("Return date must be after loan date.");
//        }
//    }
//}
