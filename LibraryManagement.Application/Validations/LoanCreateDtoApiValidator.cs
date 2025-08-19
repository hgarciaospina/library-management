using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Validations
{
    /// <summary>
    /// Asynchronous validator for API requests.
    /// Ensures required fields are present and verifies book existence in the database.
    /// ⚡ IMPORTANT: Uses the repository directly to avoid circular dependencies with services.
    /// </summary>
    public class LoanCreateDtoApiValidator : AbstractValidator<LoanCreateDto>
    {
        public LoanCreateDtoApiValidator(IGenericRepository<Book> bookRepository)
        {
            // Library must be selected
            RuleFor(l => l.LibraryId)
                .GreaterThan(0)
                .WithMessage("Please select a library.");

            // Book must be selected and exist in database
            RuleFor(l => l.BookId)
                .GreaterThan(0)
                .WithMessage("Please select a book.")
                .MustAsync(async (bookId, cancellationToken) =>
                {
                    var book = await bookRepository.GetByIdAsync(bookId);
                    return book != null;
                }).WithMessage("The selected book does not exist.");

            // Member must be selected
            RuleFor(l => l.MemberId)
                .GreaterThan(0)
                .WithMessage("Please select a member.");

            // Due date must be after today
            RuleFor(l => l.DueDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("Due date must be after today.");
        }
    }
}
