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
    /// Handles CRUD operations and fetching loans with related Book, Member, and Library data.
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly IGenericRepository<Loan> _repository;
        private readonly IMapper _mapper;

        public LoanService(IGenericRepository<Loan> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all loans without including navigation properties.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Get all loans including related Book, Member, and Library entities.
        /// LibraryName is mapped from Book.Library.Name for display purposes.
        /// </summary>
        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            // ✅ Include Book, Book.Library, and Member so LibraryName can be mapped
            var loans = await _repository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,  // Include Library navigation property
                l => l.Member
            );

            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        /// <summary>
        /// Get a single loan by ID including related Book, Member, and Library entities.
        /// This ensures that LibraryId and LibraryName are available for redirects or display.
        /// </summary>
        public async Task<LoanDto> GetByIdWithDetailsAsync(int id)
        {
            // Include Book, Book.Library, and Member
            var loans = await _repository.GetAllIncludingAsync(
                l => l.Book,
                l => l.Book.Library,
                l => l.Member
            );

            // Find the specific loan by ID
            var loanEntity = loans.FirstOrDefault(l => l.Id == id);

            // Map to LoanDto including LibraryId and LibraryName
            return _mapper.Map<LoanDto>(loanEntity);
        }

        /// <summary>
        /// Get a single loan by ID without including navigation properties.
        /// </summary>
        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _repository.GetByIdAsync(id);
            return _mapper.Map<LoanDto>(loan);
        }

        /// <summary>
        /// Create a new loan.
        /// </summary>
        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);
            await _repository.AddAsync(loan);
            return _mapper.Map<LoanDto>(loan);
        }

        /// <summary>
        /// Update an existing loan.
        /// </summary>
        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);
            await _repository.UpdateAsync(loan);
        }

        /// <summary>
        /// Delete a loan by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
