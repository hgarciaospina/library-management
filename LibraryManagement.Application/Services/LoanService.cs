using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service implementation for Loan entity.
    /// Handles CRUD operations, validation, and manages book availability.
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Loan> _loanValidator;

        public LoanService(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Book> bookRepository,
            IMapper mapper,
            IValidator<Loan> loanValidator)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _loanValidator = loanValidator;
        }

        /// <summary>
        /// Retrieves all loans without including related entities.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Retrieves all loans including Book, Member, and Library entities.
        /// Orders unreturned loans by due date descending, then by member name, then returned loans by due date ascending.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            var orderedLoans = loans
                .OrderByDescending(l => !l.ReturnDate.HasValue)   // Unreturned first
                .ThenByDescending(l => l.DueDate)                // Most recent due date
                .ThenBy(l => l.Member.LastName)                  // Member last name
                .ThenBy(l => l.Member.FirstName)                 // Member first name
                .ThenBy(l => l.ReturnDate ?? DateTime.MaxValue)  // Returned loans last
                .ToList();

            return _mapper.Map<IEnumerable<LoanDto>>(orderedLoans);
        }

        /// <summary>
        /// Retrieves a single loan by ID including related Book, Member, and Library entities.
        /// </summary>
        public async Task<LoanDto> GetByIdWithDetailsAsync(int id)
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            var loanEntity = loans.FirstOrDefault(l => l.Id == id);
            if (loanEntity == null)
                throw new ArgumentException("Loan not found.");

            return _mapper.Map<LoanDto>(loanEntity);
        }

        /// <summary>
        /// Retrieves a single loan by ID without related entities.
        /// </summary>
        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new ArgumentException("Loan not found.");

            return _mapper.Map<LoanDto>(loan);
        }

        /// <summary>
        /// Retrieves a single loan with all related details for the Details page.
        /// Includes member full name, book title & ISBN, library info, and all loan dates.
        /// </summary>
        public async Task<LoanDetailsDto?> GetDetailsByIdAsync(int id)
        {
            var loanEntity = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            ).ContinueWith(t => t.Result.FirstOrDefault(l => l.Id == id));

            if (loanEntity == null)
                return null;

            return _mapper.Map<LoanDetailsDto>(loanEntity);
        }

        /// <summary>
        /// Creates a new loan and marks the book as unavailable.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);

            var validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Loan validation failed: {errors}");
            }

            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
                throw new ArgumentException("Selected book does not exist.");

            book.IsAvailable = false;
            await _bookRepository.UpdateAsync(book);

            await _loanRepository.AddAsync(loan);

            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LibraryId = book.LibraryId;
            loanDto.LibraryName = book.Library?.Name;

            return loanDto;
        }

        /// <summary>
        /// Updates an existing loan.
        /// Only updates allowed fields and sets book availability if returned.
        /// </summary>
        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            var loan = await _loanRepository.GetByIdAsync(dto.Id);
            if (loan == null)
                throw new ArgumentException("Loan not found.");

            loan.BookId = dto.BookId;
            loan.DueDate = dto.DueDate;
            loan.ReturnDate = dto.ReturnDate;

            if (loan.ReturnDate.HasValue)
            {
                var book = await _bookRepository.GetByIdAsync(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                    await _bookRepository.UpdateAsync(book);
                }
            }

            await _loanRepository.UpdateAsync(loan);
        }

        /// <summary>
        /// Deletes a loan by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _loanRepository.DeleteAsync(id);
        }
    }
}
