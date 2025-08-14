using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class DeleteModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public DeleteModel(ILibraryService libraryService)
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
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Library == null || Library.Id == 0)
            {
                return BadRequest();
            }

            await _libraryService.DeleteAsync(Library.Id);
            return RedirectToPage("Index");
        }
    }
}
