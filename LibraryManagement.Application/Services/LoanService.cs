using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service implementation for the Loan entity.
    /// Handles CRUD operations, validation, and book availability.
    /// 
    /// ⚡ IMPORTANT:
    /// - Web (MVC/Razor): Uses <see cref="LoanCreateDtoValidator"/> (sync-only), runs automatically.
    /// - API: Uses <see cref="LoanCreateDtoApiValidator"/> and <see cref="LoanUpdateDtoApiValidator"/> (async rules allowed),
    ///   manual validation is performed in this service to avoid issues with ASP.NET's non-async pipeline.
    /// - Validation exceptions are thrown as <see cref="ValidationException"/> and handled by global middleware.
    /// </summary>
    public class LoanService : ILoanService
    {
        // Repositories
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;

        // AutoMapper for DTO <-> Entity mapping
        private readonly IMapper _mapper;

        // API validators for manual validation (async capable)
        private readonly IValidator<Loan> _loanCreateDtoApiValidator;
        private readonly IValidator<LoanUpdateDto> _loanUpdateDtoApiValidator;

        /// <summary>
        /// Constructor: injects repositories, mapper, and API validators.
        /// </summary>
        public LoanService(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Book> bookRepository,
            IMapper mapper,
            IValidator<Loan> loanCreateDtoApiValidator,
            IValidator<LoanUpdateDto> loanUpdateDtoApiValidator)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _loanCreateDtoApiValidator = loanCreateDtoApiValidator;
            _loanUpdateDtoApiValidator = loanUpdateDtoApiValidator;
        }

        /// <summary>
        /// Retrieves all loans without related entities.
        /// Lightweight operation for lists where only loan fields are needed.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Retrieves all loans including related Book, Member, and Library entities.
        /// Orders unreturned loans first by due date descending, then returned loans last.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            var orderedLoans = loans
                .OrderByDescending(l => !l.ReturnDate.HasValue)   // Unreturned loans first
                .ThenByDescending(l => l.DueDate)                // Most recent due date
                .ThenBy(l => l.Member.LastName)                  // Member last name
                .ThenBy(l => l.Member.FirstName)                 // Member first name
                .ThenBy(l => l.ReturnDate ?? DateTime.MaxValue)  // Returned loans last
                .ToList();

            return _mapper.Map<IEnumerable<LoanDto>>(orderedLoans);
        }

        /// <summary>
        /// Retrieves all loans including detailed information (LoanDetailsDto)
        /// for Index or listing views.
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
        /// Retrieves a loan by ID including Book, Member, and Library entities.
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
        /// Retrieves a loan by ID without related entities.
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
        /// Retrieves a loan by ID with full details (LoanDetailsDto).
        /// Returns null if loan is not found.
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
        /// Creates a new loan and marks the associated book as unavailable.
        /// Manual async validation is used for API requests.
        /// Validation exceptions are handled by middleware.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);

            // ✅ API manual async validation
            var validationResult = await _loanCreateDtoApiValidator.ValidateAsync(loan);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Validation failed", validationResult.Errors);
            }

            // Check if book exists
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

            await _loanRepository.AddAsync(loan);

            // Map entity back to DTO
            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LibraryId = book.LibraryId;
            loanDto.LibraryName = book.Library?.Name ?? string.Empty;

            return loanDto;
        }

        // Implementar el método GetLoansByBookIdAsync
        public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(int bookId)
        {
            var loans = await _loanRepository.GetAllAsync(l => l.BookId == bookId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }


        /// <summary>
        /// Updates an existing loan record.
        /// Validates the DTO using async API validator.
        /// Updates loan properties and book availability if returned.
        /// </summary>
        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            // ✅ API async validation
            var validationResult = await _loanUpdateDtoApiValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var loan = await _loanRepository.GetByIdAsync(dto.Id);
            if (loan == null)
                throw new KeyNotFoundException($"Loan with ID {dto.Id} not found.");

            // Update loan fields
            loan.BookId = dto.BookId;
            loan.DueDate = dto.DueDate;
            loan.ReturnDate = dto.ReturnDate;

            // If returned, mark book as available
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
