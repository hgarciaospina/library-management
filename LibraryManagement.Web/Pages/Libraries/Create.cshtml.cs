using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class CreateModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public CreateModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        // BindProperty enlaza automáticamente el formulario con el DTO
        [BindProperty]
        public LibraryCreateDto Library { get; set; }

        public void OnGet()
        {
            // Inicializar DTO para evitar nulls
            Library = new LibraryCreateDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar con FluentValidation antes de guardar
            if (!ModelState.IsValid)
            {
                return Page(); // Devuelve la página mostrando errores
            }

            // Llamada al servicio para crear la Library
            await _libraryService.CreateAsync(Library);

            return RedirectToPage("Index"); // Redirige al listado después de crear
        }
    }
}
