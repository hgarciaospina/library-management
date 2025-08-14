using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class DetailsModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public DetailsModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [BindProperty]
        public LibraryDto Library { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Library = await _libraryService.GetByIdAsync(id);

            if (Library == null)
            {
                TempData["ErrorMessage"] = "The requested library does not exist.";
                return RedirectToPage("Index"); // Redirige a la lista
            }

            return Page();
        }
    }
}
