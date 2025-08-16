using LibraryManagement.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing Loans.
    /// Provides methods for CRUD operations and fetching loans with related data.
    /// </summary>
    public interface ILoanService
    {
        /// <summary>
        /// Get all loans without including related entities.
        /// </summary>
        Task<IEnumerable<LoanDto>> GetAllAsync();

        /// <summary>
        /// Get a single loan by its ID without including related entities.
        /// </summary>
        Task<LoanDto> GetByIdAsync(int id);

        /// <summary>
        /// Get a single loan by its ID including related Book, Member, and Library entities.
        /// This ensures that LibraryId and LibraryName are available for display or redirects.
        /// </summary>
        Task<LoanDto> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Create a new loan.
        /// </summary>
        Task<LoanDto> CreateAsync(LoanCreateDto dto);

        /// <summary>
        /// Get all loans including related Book and Member entities.
        /// Useful for displaying in views with full details.
        /// </summary>
        Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync();

        /// <summary>
        /// Update an existing loan.
        /// </summary>
        Task UpdateAsync(LoanUpdateDto dto);

        /// <summary>
        /// Delete a loan by ID.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
