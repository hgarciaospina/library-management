using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly IBookService _bookService;
        public CreateModel(IBookService bookService) => _bookService = bookService;

        [BindProperty]
        public BookCreateDto Book { get; set; } = new BookCreateDto();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _bookService.CreateAsync(Book);
            return RedirectToPage("Index");
        }
    }
}
