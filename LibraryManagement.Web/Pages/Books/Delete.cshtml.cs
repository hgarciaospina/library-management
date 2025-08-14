using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;
        public DeleteModel(IBookService bookService) => _bookService = bookService;

        [BindProperty]
        public BookDto Book { get; set; } = new BookDto();

        public async Task OnGetAsync(int id) => Book = await _bookService.GetByIdAsync(id);

        public async Task<IActionResult> OnPostAsync()
        {
            await _bookService.DeleteAsync(Book.Id);
            return RedirectToPage("Index");
        }
    }
}
