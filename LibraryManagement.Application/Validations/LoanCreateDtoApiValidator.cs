using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// Asynchronous validator for API requests.
    /// Ensures required fields and verifies book existence in the database.
    /// </summary>
    public class LoanCreateDtoApiValidator : AbstractValidator<LoanCreateDto>
    {
        public LoanCreateDtoApiValidator(IBookService bookService)
        {
            RuleFor(l => l.LibraryId)
                .GreaterThan(0)
                .WithMessage("Please select a library.");

            RuleFor(l => l.BookId)
                .GreaterThan(0)
                .WithMessage("Please select a book.")
                .MustAsync(async (bookId, cancellation) =>
                {
                    var book = await bookService.GetByIdAsync(bookId);
                    return book != null;
                }).WithMessage("The selected book does not exist.");

            RuleFor(l => l.MemberId)
                .GreaterThan(0)
                .WithMessage("Please select a member.");

            RuleFor(l => l.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Due date must be after today.");
        }
    }
}
