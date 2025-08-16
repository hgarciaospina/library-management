using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service implementation for Loan entity.
    /// Handles CRUD operations and ensures books are marked unavailable when a loan is created.
    /// Now integrates FluentValidation to ensure entities are valid before persisting.
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
            IValidator<Loan> loanValidator) // Inject validator
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _loanValidator = loanValidator;
        }

        /// <summary>
        /// Get all loans without including navigation properties.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Get all loans including related Book, Book.Library, and Member entities.
        /// LibraryName is mapped from Book.Library.Name for display purposes.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Get a single loan by ID including related Book, Member, and Library entities.
        /// Ensures LibraryId and LibraryName are available.
        /// </summary>
        public async Task<LoanDto> GetByIdWithDetailsAsync(int id)
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            var loanEntity = loans.FirstOrDefault(l => l.Id == id);
            return _mapper.Map<LoanDto>(loanEntity);
        }

        /// <summary>
        /// Get a single loan by ID without including navigation properties.
        /// </summary>
        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            return _mapper.Map<LoanDto>(loan);
        }

        /// <summary>
        /// Create a new loan and mark the related book as unavailable.
        /// Validation is executed before persisting to avoid null references or invalid state.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            // Map DTO to Loan entity
            var loan = _mapper.Map<Loan>(dto);

            // Execute FluentValidation rules
            var validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid)
            {
                // Collect validation messages and throw exception
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Loan validation failed: {errors}");
            }

            // Get the book entity to mark it as unavailable
            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                throw new ArgumentException("Selected book does not exist.");
            }

            // Mark book as unavailable
            book.IsAvailable = false;
            await _bookRepository.UpdateAsync(book);

            // Save the loan
            await _loanRepository.AddAsync(loan);

            // Return mapped LoanDto including LibraryId for redirection
            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LibraryId = book.LibraryId;
            loanDto.LibraryName = book.Library?.Name;

            return loanDto;
        }

        /// <summary>
        /// Update an existing loan.
        /// </summary>
        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);

            // Validate before updating
            var validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Loan validation failed: {errors}");
            }

            await _loanRepository.UpdateAsync(loan);
        }

        /// <summary>
        /// Delete a loan by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _loanRepository.DeleteAsync(id);
        }
    }
}
