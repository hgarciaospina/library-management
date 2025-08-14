using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly IBookService _bookService;
        public DetailsModel(IBookService bookService) => _bookService = bookService;

        public BookDto Book { get; set; } = new BookDto();

        public async Task OnGetAsync(int id) => Book = await _bookService.GetByIdAsync(id);
    }
}
