using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoMapper;

namespace LibraryManagement.Web.Pages.Libraries
{
    public class IndexModel : PageModel
    {
        private readonly ILibraryService _libraryService;

        public IndexModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public List<LibraryDto> Libraries { get; set; } = new();

        public async Task OnGetAsync()
        {
            Libraries = (await _libraryService.GetAllAsync()).ToList();
        }
    }
}
