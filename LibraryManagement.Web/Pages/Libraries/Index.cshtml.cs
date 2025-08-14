using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibraryManagement.Core.Entities;
using LibraryManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class IndexModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public IndexModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IList<Library> Libraries { get; set; } = new List<Library>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var result = await _libraryService.GetAllAsync();

                if (result != null)
                {
                    // Mapear de DTO a entidad solo con lo que se necesita mostrar
                    Libraries = result.Select(dto => new Library
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Address = dto.Address,
                    }).ToList();
                }
                else
                {
                    Libraries = new List<Library>();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while loading libraries: {ex.Message}";
                Libraries = new List<Library>();
            }

            return Page();
        }
    }
}
