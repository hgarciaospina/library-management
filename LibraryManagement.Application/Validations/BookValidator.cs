using FluentValidation;
using LibraryManagement.Core.Entities;
using System;

namespace LibraryManagement.Application.Validations
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(b => b.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author cannot exceed 100 characters.");

            RuleFor(b => b.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Matches(@"^\d{3}-\d{10}$").WithMessage("ISBN must be in the format 000-0000000000.");

            RuleFor(b => b.PublicationYear)
                .InclusiveBetween(1500, DateTime.Now.Year).WithMessage($"Publication year must be between 1500 and {DateTime.Now.Year}.");

            RuleFor(b => b.LibraryId)
                .GreaterThan(0).WithMessage("Library must be selected.");
        }
    }
}
