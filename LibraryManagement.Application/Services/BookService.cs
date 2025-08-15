using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;


namespace LibraryManagement.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _repository;
        private readonly IMapper _mapper;

        public BookService(IGenericRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> GetByIdAsync(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            return _mapper.Map<BookDto>(book);
        }

        // Get EF tracked entity by id
        public async Task<Book> GetEntityByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id); // returns the entity tracked by EF
        }
        public async Task<BookDto> CreateAsync(BookCreateDto dto)
        {
            var book = _mapper.Map<Book>(dto);
            await _repository.AddAsync(book);
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateAsync(BookUpdateDto dto)
        {
            var book = _mapper.Map<Book>(dto);
            await _repository.UpdateAsync(book);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task SaveEntityAsync(Book entity)
        {
            await _repository.SaveChangesAsync(); // EF already tracks the entity
        }
    }
}
