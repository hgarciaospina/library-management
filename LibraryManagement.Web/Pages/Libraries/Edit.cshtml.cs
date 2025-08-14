using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class EditModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public EditModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [BindProperty]
        public LibraryUpdateDto Library { get; set; } = new LibraryUpdateDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _libraryService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            // Mapear manualmente los valores al UpdateDto
            Library.Id = dto.Id;
            Library.Name = dto.Name;
            Library.Address = dto.Address;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _libraryService.UpdateAsync(Library);
            return RedirectToPage("Index");
        }
    }
}
