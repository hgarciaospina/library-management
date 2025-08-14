using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly IBookService _bookService;
        public DetailsModel(IBookService bookService) => _bookService = bookService;

        [BindProperty]
        public BookDto Book { get; set; } = new BookDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            Book = book;
            return Page();
        }
    }
}
