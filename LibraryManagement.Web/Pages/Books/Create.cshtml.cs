using AutoMapper;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace LibraryManagement.Web.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ILibraryService _libraryService;
        private readonly IMapper _mapper;

        public CreateModel(IBookService bookService, ILibraryService libraryService, IMapper mapper)
        {
            _bookService = bookService;
            _libraryService = libraryService;
            _mapper = mapper;
        }

        [BindProperty]
        public BookCreateDto Book { get; set; } = new BookCreateDto
        {
            PublicationYear = System.DateTime.Now.Year
        };

        public List<SelectListItem> LibraryList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadLibrariesAsync();
            return Page();
        }

        private async Task LoadLibrariesAsync()
        {
            var libraries = await _libraryService.GetAllAsync();
            LibraryList = libraries.Select(lib => new SelectListItem
            {
                Value = lib.Id.ToString(),
                Text = lib.Name
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Si ModelState es inválido, se regresan los errores automáticamente
            if (!ModelState.IsValid)
            {
                await LoadLibrariesAsync();
                return Page();
            }

            // Guardar libro
            await _bookService.CreateAsync(Book);

            return RedirectToPage("Index");
        }
    }
}
