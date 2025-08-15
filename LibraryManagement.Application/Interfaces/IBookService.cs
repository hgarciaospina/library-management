using LibraryManagement.Application.DTOs;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto> GetByIdAsync(int id);
        Task<BookDto> CreateAsync(BookCreateDto dto);
        Task UpdateAsync(BookUpdateDto dto);
        Task DeleteAsync(int id);
        // NEW: Get the actual EF tracked entity
        Task<Book> GetEntityByIdAsync(int id);
        Task SaveEntityAsync(Book entity);
    }
}
