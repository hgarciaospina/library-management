using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDto>> GetAllAsync();
        Task<LoanDto> GetByIdAsync(int id);
        Task<LoanDto> CreateAsync(LoanCreateDto dto);
        Task UpdateAsync(LoanUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
