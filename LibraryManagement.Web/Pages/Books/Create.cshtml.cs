using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace LibraryManagement.Web.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ILibraryService _libraryService;

        public CreateModel(IBookService bookService, ILibraryService libraryService)
        {
            _bookService = bookService;
            _libraryService = libraryService;
        }

        // Bind property to hold the form data
        [BindProperty]
        public BookCreateDto Book { get; set; } = new BookCreateDto
        {
            PublicationYear = DateTime.Now.Year
        };

        // List of libraries for dropdown
        public List<SelectListItem> LibraryList { get; set; } = new();

        // GET handler: load libraries for the select dropdown
        public async Task<IActionResult> OnGetAsync()
        {
            await LoadLibrariesAsync();
            return Page();
        }

        // POST handler: validate and create a new book
        public async Task<IActionResult> OnPostAsync()
        {
            // Run FluentValidation + Razor validation
            if (!ModelState.IsValid)
            {
                await LoadLibrariesAsync();
                return Page();
            }

            // Additional validation: ensure a LibraryId is selected
            if (!Book.LibraryId.HasValue || Book.LibraryId.Value <= 0)
            {
                ModelState.AddModelError("Book.LibraryId", "Please select a library.");
                await LoadLibrariesAsync();
                return Page();
            }

            // Safe to send the value to the service
            await _bookService.CreateAsync(Book);

            return RedirectToPage("Index");
        }

        // Helper method to load libraries into the dropdown
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
