using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<LibraryDto>> GetAllAsync();
        Task<LibraryDto> GetByIdAsync(int id);
        Task<LibraryDto> CreateAsync(LibraryCreateDto dto);
        Task UpdateAsync(LibraryUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
