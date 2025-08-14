using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto> GetByIdAsync(int id);
        Task<BookDto> CreateAsync(BookCreateDto dto);
        Task UpdateAsync(BookUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
