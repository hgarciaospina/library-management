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
    /// Handles CRUD operations, validation, and manages book availability.
    /// 
    /// ⚡ IMPORTANT:
    /// - For the Web (MVC/Razor): Uses <see cref="LoanCreateDtoValidator"/> (sync-only).
    ///   This runs automatically with ASP.NET model binding.
    /// - For the API: Uses <see cref="LoanCreateDtoApiValidator"/> (can contain async rules).
    ///   Automatic validation is DISABLED for Loans, and validation is run MANUALLY in this service.
    /// - Validation errors are wrapped in <see cref="ValidationException"/> and handled by
    ///   global middleware to return friendly messages to the client.
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Loan> _loanValidator; // ✅ API async validator

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
        /// Orders unreturned loans by due date descending, then by member name,
        /// then returned loans by due date ascending.
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
        /// Retrieves all loans with detailed information for the Index or listing view.
        /// Maps to LoanDetailsDto which includes library, book, member, and dates.
        /// </summary>
        public async Task<IEnumerable<LoanDetailsDto>> GetAllDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            return _mapper.Map<IEnumerable<LoanDetailsDto>>(loans);
        }

        /// <summary>
        /// Retrieves a single loan by ID including related Book, Member, and Library entities.
        /// Throws ArgumentException if the loan does not exist.
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
        /// Throws ArgumentException if the loan does not exist.
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
        /// Returns null if not found.
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
        /// 
        /// ✅ API: Manual validation with <see cref="LoanCreateDtoApiValidator"/> (async rules allowed).
        /// ✅ Web: Validation handled automatically with <see cref="LoanCreateDtoValidator"/> (sync only).
        /// 
        /// Returns friendly validation messages handled by global middleware.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            // Map DTO to Loan entity
            var loan = _mapper.Map<Loan>(dto);

            // ✅ API: manual async validation
            var validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Validation failed", validationResult.Errors);
            }

            // ✅ Additional business validation: book existence
            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                var failures = new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure(nameof(dto.BookId), "The selected book does not exist.")
                };
                throw new ValidationException("Validation failed", failures);
            }

            // Mark book as unavailable
            book.IsAvailable = false;
            await _bookRepository.UpdateAsync(book);

            // Save loan
            await _loanRepository.AddAsync(loan);

            // Map entity back to DTO for response
            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LibraryId = book.LibraryId;
            loanDto.LibraryName = book.Library?.Name ?? string.Empty;

            return loanDto;
        }

        /// <summary>
        /// Updates an existing loan.
        /// Updates book availability if the loan is returned.
        /// Throws ArgumentException if the loan does not exist.
        /// </summary>
        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            var loan = await _loanRepository.GetByIdAsync(dto.Id);
            if (loan == null)
                throw new ArgumentException("Loan not found.");

            loan.BookId = dto.BookId;
            loan.DueDate = dto.DueDate;
            loan.ReturnDate = dto.ReturnDate;

            // If loan is returned, mark the book as available
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
        /// Throws ArgumentException if the loan does not exist.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _loanRepository.DeleteAsync(id);
        }
    }
}
