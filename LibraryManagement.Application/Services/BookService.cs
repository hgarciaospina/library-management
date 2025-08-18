using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Servicio de manejo de libros (Book).
    /// Incluye CRUD y lectura con datos de navegación (Library) usando AutoMapper.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _repository;
        private readonly IMapper _mapper;

        public BookService(IGenericRepository<Book> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los libros incluyendo el nombre de la biblioteca asociada.
        /// </summary>
        /// <returns>Lista de BookDto con LibraryName completo</returns>
        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            // Trae todos los libros incluyendo la entidad Library para poder mapear LibraryName
            var books = await _repository.GetAllIncludingAsync(b => b.Library);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        /// <summary>
        /// Obtiene un libro por Id incluyendo el nombre de la biblioteca.
        /// </summary>
        /// <param name="id">Id del libro</param>
        /// <returns>BookDto con LibraryName completo</returns>
        public async Task<BookDto> GetByIdAsync(int id)
        {
            var books = await _repository.GetAllIncludingAsync(b => b.Library);
            var book = books.FirstOrDefault(b => b.Id == id)
                       ?? throw new Exception($"Book with Id {id} not found");
            return _mapper.Map<BookDto>(book);
        }

        /// <summary>
        /// Obtiene la entidad EF de Book para operaciones de edición o manipulación directa.
        /// EF ya la trackea.
        /// </summary>
        public async Task<Book> GetEntityByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Crea un nuevo libro y devuelve el BookDto resultante.
        /// </summary>
        public async Task<BookDto> CreateAsync(BookCreateDto dto)
        {
            var book = _mapper.Map<Book>(dto);
            await _repository.AddAsync(book);
            return _mapper.Map<BookDto>(book);
        }

        /// <summary>
        /// Actualiza un libro existente con la información de BookUpdateDto.
        /// </summary>
        public async Task UpdateAsync(BookUpdateDto dto)
        {
            var book = _mapper.Map<Book>(dto);
            await _repository.UpdateAsync(book);
        }

        /// <summary>
        /// Elimina un libro por Id.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Guarda los cambios en la entidad EF trackeada.
        /// </summary>
        public async Task SaveEntityAsync(Book entity)
        {
            await _repository.SaveChangesAsync();
        }
    }
}
