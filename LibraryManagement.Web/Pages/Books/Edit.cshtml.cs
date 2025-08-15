using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace LibraryManagement.Web.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ILibraryService _libraryService;

        public EditModel(IBookService bookService, ILibraryService libraryService)
        {
            _bookService = bookService;
            _libraryService = libraryService;
        }

        [BindProperty]
        public BookUpdateDto Book { get; set; } = new BookUpdateDto();

        public List<SelectListItem> LibraryList { get; set; } = new();

        // GET: load existing book
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _bookService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            // Map manually to BookUpdateDto
            Book.Id = dto.Id;
            Book.Title = dto.Title;
            Book.Author = dto.Author;
            Book.ISBN = dto.ISBN;
            Book.PublicationYear = dto.PublicationYear == 0 ? DateTime.Now.Year : dto.PublicationYear;
            Book.LibraryId = dto.LibraryId;
            Book.IsAvailable = dto.IsAvailable;

            await LoadLibrariesAsync();
            return Page();
        }

        // POST: validate and update using DTO directly
        public async Task<IActionResult> OnPostAsync()
        {
            // FluentValidation via ModelState
            if (!ModelState.IsValid)
            {
                await LoadLibrariesAsync();
                return Page();
            }

            // Update using BookUpdateDto directly
            await _bookService.UpdateAsync(Book);

            return RedirectToPage("Index");
        }

        private async Task LoadLibrariesAsync()
        {
            var libraries = await _libraryService.GetAllAsync();
            LibraryList = libraries.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            }).ToList();
        }
    }
}
