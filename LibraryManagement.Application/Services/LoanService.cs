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
    /// Handles CRUD operations and book availability.
    ///
    /// ⚡ IMPORTANT:
    /// - Web (MVC/Razor): Uses <see cref="LoanCreateDtoValidator"/> (sync-only), runs automatically.
    /// - API: No validation is executed automatically.
    /// </summary>
    public class LoanService : ILoanService
    {
        #region Fields

        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor: injects repositories and mapper.
        /// </summary>
        public LoanService(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Book> bookRepository,
            IMapper mapper)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
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

            // No validation here - Web will validate automatically via sync validators

            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                throw new ArgumentException("The selected book does not exist.");
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

        public async Task<IEnumerable<LoanDto>> GetLoansByMemberIdAsync(int memberId)
        {
            var loans = await _loanRepository.GetAllAsync(l => l.MemberId == memberId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        #endregion

        #region Update Methods

        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            // No validation here - Web will validate automatically via sync validators

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
