using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service for managing books (Book).
    /// Includes CRUD operations and reading with navigation data (Library) using AutoMapper.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _repository;  // Repository for Book entity
        private readonly ILoanService _loanService;  // Service to manage Loans
        private readonly IMapper _mapper;  // AutoMapper for DTO to entity mapping

        /// <summary>
        /// Constructor: Injects repository, loan service, and AutoMapper.
        /// </summary>
        public BookService(IGenericRepository<Book> repository, ILoanService loanService, IMapper mapper)
        {
            _repository = repository;
            _loanService = loanService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all books including the associated Library name.
        /// </summary>
        /// <returns>List of BookDto with complete LibraryName</returns>
        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            // Fetches all books including the Library entity to map LibraryName
            var books = await _repository.GetAllIncludingAsync(b => b.Library);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        /// <summary>
        /// Retrieves a book by Id including the associated Library name.
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <returns>BookDto with complete LibraryName</returns>
        public async Task<BookDto> GetByIdAsync(int id)
        {
            // Fetch all books including the Library entity
            var books = await _repository.GetAllIncludingAsync(b => b.Library);

            // Find the specific book by Id
            var book = books.FirstOrDefault(b => b.Id == id)
                       ?? throw new Exception($"Book with Id {id} not found");

            return _mapper.Map<BookDto>(book);
        }

        /// <summary>
        /// Retrieves the EF entity of Book for direct editing or manipulation.
        /// EF tracks the entity for changes.
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <returns>Book entity</returns>
        public async Task<Book> GetEntityByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Retrieves all loans for a specific book by its Id.
        /// </summary>
        /// <param name="bookId">Id of the book</param>
        /// <returns>List of LoanDto associated with the book</returns>
        public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(int bookId)
        {
            // Fetch the loans using LoanService
            var loans = await _loanService.GetLoansByBookIdAsync(bookId);
            return loans;  // Return the loans as LoanDto
        }

        /// <summary>
        /// Creates a new book and returns the resulting BookDto.
        /// </summary>
        /// <param name="dto">The BookCreateDto containing book data</param>
        /// <returns>Created BookDto</returns>
        public async Task<BookDto> CreateAsync(BookCreateDto dto)
        {
            // Map the BookCreateDto to Book entity
            var book = _mapper.Map<Book>(dto);

            // Add the book to the repository
            await _repository.AddAsync(book);

            // Map the created Book entity back to BookDto
            return _mapper.Map<BookDto>(book);
        }

        /// <summary>
        /// Updates an existing book with data from BookUpdateDto.
        /// </summary>
        /// <param name="dto">The BookUpdateDto containing updated data</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task UpdateAsync(BookUpdateDto dto)
        {
            // Map the BookUpdateDto to Book entity
            var book = _mapper.Map<Book>(dto);

            // Update the book in the repository
            await _repository.UpdateAsync(book);
        }

        /// <summary>
        /// Deletes a book by Id.
        /// </summary>
        /// <param name="id">Id of the book to be deleted</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task DeleteAsync(int id)
        {
            // Delete the book by Id from the repository
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Saves changes to the tracked entity in EF.
        /// </summary>
        /// <param name="entity">The Book entity to save</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task SaveEntityAsync(Book entity)
        {
            // Save changes to the EF context
            await _repository.SaveChangesAsync();
        }
    }
}
