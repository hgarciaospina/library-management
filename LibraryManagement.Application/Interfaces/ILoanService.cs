using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDto>> GetAllAsync();
        Task<LoanDto> GetByIdAsync(int id);
        Task<LoanDto> CreateAsync(LoanCreateDto dto);

        // ✅ New: Get all loans including related Book and Member
        Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync();
        Task UpdateAsync(LoanUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
