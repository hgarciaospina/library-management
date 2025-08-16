using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service implementation for Loan entity.
    /// Handles CRUD operations and ensures books are marked unavailable when a loan is created.
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IMapper _mapper;

        public LoanService(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Book> bookRepository,
            IMapper mapper)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
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
        /// Both operations occur in the same service method to ensure consistency.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            // Map DTO to Loan entity
            var loan = _mapper.Map<Loan>(dto);

            // Get the book entity to mark it as unavailable
            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {dto.BookId} not found.");
            }

            // Mark book as unavailable
            book.IsAvailable = false;

            // Save the book update
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
