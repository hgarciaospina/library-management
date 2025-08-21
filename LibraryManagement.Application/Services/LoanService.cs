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
    /// Handles CRUD operations and manages book availability.
    ///
    /// ⚡ IMPORTANT:
    /// - Web (MVC/Razor): Uses <see cref="LoanCreateDtoValidator"/> (sync-only), runs automatically.
    /// - API: No validation is executed automatically (must be added manually).
    /// </summary>
    public class LoanService : ILoanService
    {
        #region Fields

        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IGenericRepository<Library> _libraryRepository;
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor: injects repositories and AutoMapper instance.
        /// </summary>
        public LoanService(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Book> bookRepository,
            IGenericRepository<Library> libraryRepository,
            IGenericRepository<Member> memberRepository,
            IMapper mapper)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _libraryRepository = libraryRepository;
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        #endregion

        #region Read Methods

        /// <summary>
        /// Returns all loans without additional details.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Returns all loans with related book, library, and member details.
        /// Orders loans by active status and due date.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            var loans = await _loanRepository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            var orderedLoans = loans
                .OrderByDescending(l => !l.ReturnDate.HasValue)   // Active loans first
                .ThenByDescending(l => l.DueDate)
                .ThenBy(l => l.Member.LastName)
                .ThenBy(l => l.Member.FirstName)
                .ThenBy(l => l.ReturnDate ?? DateTime.MaxValue)
                .ToList();

            return _mapper.Map<IEnumerable<LoanDto>>(orderedLoans);
        }

        /// <summary>
        /// Returns all loans mapped to LoanDetailsDto.
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
        /// Returns a single loan by ID with related details.
        /// Throws exception if not found.
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
        /// Returns a single loan by ID without details.
        /// </summary>
        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new ArgumentException("Loan not found.");

            return _mapper.Map<LoanDto>(loan);
        }

        /// <summary>
        /// Returns a single loan with details, or null if not found.
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

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates a new loan. Validates that the book, library, and member exist.
        /// Also marks the book as unavailable.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);

            // Validate book
            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
                throw new ArgumentException("The selected book does not exist.");

            // Validate library
            var library = await _libraryRepository.GetByIdAsync(dto.LibraryId);
            if (library == null)
                throw new ArgumentException("The selected library does not exist.");

            // Validate member
            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member == null)
                throw new ArgumentException("The selected member does not exist.");

            // Mark book unavailable
            book.IsAvailable = false;
            await _bookRepository.UpdateAsync(book);

            // Save loan
            await _loanRepository.AddAsync(loan);

            var loanDto = _mapper.Map<LoanDto>(loan);
            loanDto.LibraryId = book.LibraryId;
            loanDto.LibraryName = book.Library?.Name ?? string.Empty;

            return loanDto;
        }

        /// <summary>
        /// Returns all loans associated with a specific book.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(int bookId)
        {
            var loans = await _loanRepository.GetAllAsync(l => l.BookId == bookId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Returns all loans associated with a specific member.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetLoansByMemberIdAsync(int memberId)
        {
            var loans = await _loanRepository.GetAllAsync(l => l.MemberId == memberId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates an existing loan. 
        /// If the loan is returned, the book is marked available again.
        /// </summary>
        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            //validations that do not affect the book update in the loan.

            var loan = await _loanRepository.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"Loan with ID {dto.Id} not found.");

            var book = await _bookRepository.GetByIdAsync(dto.BookId)
                ?? throw new KeyNotFoundException($"Book with ID {dto.BookId} not found.");

            var member = await _memberRepository.GetByIdAsync(dto.MemberId)
               ?? throw new KeyNotFoundException($"Member with ID {dto.MemberId} not found.");

            var library = await _libraryRepository.GetByIdAsync(dto.LibraryId)
             ?? throw new KeyNotFoundException($"Library with ID {dto.LibraryId} not found.");


            // Update loan fields
            loan.BookId = dto.BookId;
            loan.DueDate = dto.DueDate;
            loan.ReturnDate = dto.ReturnDate;

            if (loan.ReturnDate.HasValue)
            {
                // Mark book as available if loan is returned
                book.IsAvailable = true;
                await _bookRepository.UpdateAsync(book);
            }

            await _loanRepository.UpdateAsync(loan);
        }

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes a loan by its ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _loanRepository.DeleteAsync(id);
        }

        #endregion
    }
}
