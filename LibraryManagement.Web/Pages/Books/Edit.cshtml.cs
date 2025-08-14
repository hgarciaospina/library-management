using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoMapper;

namespace LibraryManagement.Web.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public EditModel(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [BindProperty]
        public BookUpdateDto Book { get; set; } = new BookUpdateDto();

        public async Task OnGetAsync(int id)
        {
            var bookDto = await _bookService.GetByIdAsync(id);
            Book = _mapper.Map<BookUpdateDto>(bookDto);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _bookService.UpdateAsync(Book);
            return RedirectToPage("Index");
        }
    }
}
