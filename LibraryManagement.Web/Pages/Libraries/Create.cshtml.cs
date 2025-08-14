using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class CreateModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public CreateModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [BindProperty]
        public LibraryCreateDto Library { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _libraryService.CreateAsync(Library);
            return RedirectToPage("Index");
        }
    }
}
