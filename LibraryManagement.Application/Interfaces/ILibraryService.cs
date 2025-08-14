using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<LibraryDto>> GetAllAsync();
        Task<LibraryDto> GetByIdAsync(int id);
        Task<LibraryDto> CreateAsync(LibraryCreateDto libraryCreateDto);  
        Task UpdateAsync(LibraryUpdateDto libraryUpdateDto);
        Task DeleteAsync(int id);
    }
}
