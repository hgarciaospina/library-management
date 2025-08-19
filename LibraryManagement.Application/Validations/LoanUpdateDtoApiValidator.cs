using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Core.Entities;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// API Validator for updating a Loan.
    /// Performs asynchronous validation to ensure referenced entities exist (Loan, Library, Book, Member)
    /// and that date constraints are respected.
    /// 
    /// ⚡ IMPORTANT:
    /// - Does NOT depend on ILoanService anymore to prevent circular dependencies.
    /// - Injects only the repositories needed to check existence.
    /// - Async rules are fully supported for API validation.
    /// </summary>
    public class LoanUpdateDtoApiValidator : AbstractValidator<LoanUpdateDto>
    {
        public LoanUpdateDtoApiValidator(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<LibraryManagement.Core.Entities.Library> libraryRepository,
            IGenericRepository<Book> bookRepository,
            IGenericRepository<LibraryManagement.Core.Entities.Member> memberRepository)
        {
            // ==============================
            // Loan Id must exist
            // ==============================
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Loan Id must be greater than zero.")
                .MustAsync(async (id, cancellation) =>
                {
                    var loan = await loanRepository.GetByIdAsync(id);
                    return loan != null;
                })
                .WithMessage("The specified loan does not exist.");

            // ==============================
            // LibraryId must exist
            // ==============================
            RuleFor(x => x.LibraryId)
                .GreaterThan(0)
                .WithMessage("LibraryId is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    var library = await libraryRepository.GetByIdAsync(id);
                    return library != null;
                })
                .WithMessage("The selected library does not exist.");

            // ==============================
            // BookId must exist
            // ==============================
            RuleFor(x => x.BookId)
                .GreaterThan(0)
                .WithMessage("BookId is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    var book = await bookRepository.GetByIdAsync(id);
                    return book != null;
                })
                .WithMessage("The selected book does not exist.");

            // ==============================
            // MemberId must exist
            // ==============================
            RuleFor(x => x.MemberId)
                .GreaterThan(0)
                .WithMessage("MemberId is required.")
                .MustAsync(async (id, cancellation) =>
                {
                    var member = await memberRepository.GetByIdAsync(id);
                    return member != null;
                })
                .WithMessage("The selected member does not exist.");

            // ==============================
            // DueDate required
            // ==============================
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required.");

            // ==============================
            // ReturnDate constraints
            // - Must be >= LoanDate
            // - Cannot be in the future
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
