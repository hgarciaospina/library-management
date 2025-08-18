using LibraryManagement.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing Loans.
    /// Provides methods for CRUD operations and fetching loans with related entities.
    /// </summary>
    public interface ILoanService
    {
        /// <summary>
        /// Retrieves all loans without including related entities.
        /// Useful for lightweight operations where related data is not needed.
        /// </summary>
        /// <returns>A collection of LoanDto representing all loans.</returns>
        Task<IEnumerable<LoanDto>> GetAllAsync();

        /// <summary>
        /// Retrieves a single loan by its unique ID without related entities.
        /// </summary>
        /// <param name="id">The unique identifier of the loan.</param>
        /// <returns>A LoanDto representing the loan if found; otherwise, null.</returns>
        Task<LoanDto?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a single loan by its unique ID including related Book, Member, and Library entities.
        /// Ensures LibraryId and LibraryName are available for display or redirects.
        /// </summary>
        /// <param name="id">The unique identifier of the loan.</param>
        /// <returns>A LoanDto representing the loan with related details.</returns>
        Task<LoanDto?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Creates a new loan record.
        /// </summary>
        /// <param name="dto">The DTO containing data needed to create the loan.</param>
        /// <returns>The created LoanDto including its generated ID.</returns>
        Task<LoanDto> CreateAsync(LoanCreateDto dto);

        /// <summary>
        /// Retrieves all loans including related Book and Member entities.
        /// Useful for displaying full details in views.
        /// </summary>
        /// <returns>A collection of LoanDto representing all loans with full details.</returns>
        Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync();

        /// <summary>
        /// Updates an existing loan record.
        /// </summary>
        /// <param name="dto">The DTO containing updated loan data.</param>
        Task UpdateAsync(LoanUpdateDto dto);

        /// <summary>
        /// Deletes a loan by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the loan to delete.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Retrieves a single loan with full details for the Details page.
        /// Includes member full name, book title & ISBN, library info, and all loan dates.
        /// </summary>
        /// <param name="id">The unique identifier of the loan.</param>
        /// <returns>A LoanDetailsDto if found; otherwise, null.</returns>
        Task<LoanDetailsDto?> GetDetailsByIdAsync(int id);
    }
}
