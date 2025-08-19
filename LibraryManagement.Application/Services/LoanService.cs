using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validations;
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
        #region Fields

        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IMapper _mapper;

        // Async DTO validators for API usage
        private readonly LoanCreateDtoApiValidator _loanCreateDtoApiValidator;
        private readonly LoanUpdateDtoApiValidator _loanUpdateDtoApiValidator;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor: injects repositories, mapper, and API validators.
        /// </summary>
        public LoanService(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Book> bookRepository,
            IMapper mapper,
            LoanCreateDtoApiValidator loanCreateDtoApiValidator,
            LoanUpdateDtoApiValidator loanUpdateDtoApiValidator)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _loanCreateDtoApiValidator = loanCreateDtoApiValidator;
            _loanUpdateDtoApiValidator = loanUpdateDtoApiValidator;
        }

        #endregion

        #region Read Methods

        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            var orderedLoans = loans
                .OrderByDescending(l => !l.ReturnDate.HasValue)
                .ThenByDescending(l => l.DueDate)
                .ThenBy(l => l.Member.LastName)
                .ThenBy(l => l.Member.FirstName)
                .ThenBy(l => l.ReturnDate ?? DateTime.MaxValue)
                .ToList();

            return _mapper.Map<IEnumerable<LoanDto>>(orderedLoans);
        }

        public async Task<IEnumerable<LoanDetailsDto>> GetAllDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            return _mapper.Map<IEnumerable<LoanDetailsDto>>(loans);
        }

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

        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new ArgumentException("Loan not found.");

            return _mapper.Map<LoanDto>(loan);
        }

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

        #endregion

        #region Create Methods

        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);

            var validationResult = await _loanCreateDtoApiValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException("Validation failed", validationResult.Errors);

            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                var failures = new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure(nameof(dto.BookId), "The selected book does not exist.")
                };
                throw new ValidationException("Validation failed", failures);
            }

            book.IsAvailable = false;
            await _bookRepository.UpdateAsync(book);

            await _loanRepository.AddAsync(loan);

            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LibraryId = book.LibraryId;
            loanDto.LibraryName = book.Library?.Name ?? string.Empty;

            return loanDto;
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(int bookId)
        {
            var loans = await _loanRepository.GetAllAsync(l => l.BookId == bookId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Retrieves all loans associated with a specific member.
        /// Useful to check if a member has active loans before deletion.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>A collection of LoanDto associated with the member.</returns>
        public async Task<IEnumerable<LoanDto>> GetLoansByMemberIdAsync(int memberId)
        {
            var loans = await _loanRepository.GetAllAsync(l => l.MemberId == memberId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        #endregion

        #region Update Methods

        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            var validationResult = await _loanUpdateDtoApiValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var loan = await _loanRepository.GetByIdAsync(dto.Id);
            if (loan == null)
                throw new KeyNotFoundException($"Loan with ID {dto.Id} not found.");

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

        #endregion

        #region Delete Methods

        public async Task DeleteAsync(int id)
        {
            await _loanRepository.DeleteAsync(id);
        }

        #endregion
    }
}
