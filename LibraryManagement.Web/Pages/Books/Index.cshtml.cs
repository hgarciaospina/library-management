using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        public IndexModel(IBookService bookService) => _bookService = bookService;

        public IList<BookDto> Books { get; set; } = new List<BookDto>();

        public async Task OnGetAsync()
        {
            Books = (await _bookService.GetAllAsync()).ToList();
        }
    }
}
