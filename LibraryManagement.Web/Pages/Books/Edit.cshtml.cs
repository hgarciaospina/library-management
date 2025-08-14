using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly IBookService _bookService;
        public EditModel(IBookService bookService) => _bookService = bookService;

        [BindProperty]
        public BookUpdateDto Book { get; set; } = new BookUpdateDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            Book = new BookUpdateDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublicationYear = book.PublicationYear,
                IsAvailable = book.IsAvailable
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _bookService.UpdateAsync(Book);
            return RedirectToPage("Index");
        }
    }
}
