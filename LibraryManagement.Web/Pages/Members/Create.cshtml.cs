using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly ILibraryService _libraryService;

        public CreateModel(IMemberService memberService, ILibraryService libraryService)
        {
            _memberService = memberService;
            _libraryService = libraryService;
        }

        [BindProperty]
        public MemberCreateDto Member { get; set; } = new MemberCreateDto();

        public List<SelectListItem> LibraryList { get; set; } = new();

        public async Task OnGetAsync()
        {
            var libraries = await _libraryService.GetAllAsync();
            LibraryList = libraries
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLibrariesAsync(); // Reload in case of validation errors
                return Page();
            }

            await _memberService.CreateAsync(Member);
            return RedirectToPage("Index");
        }

        private async Task LoadLibrariesAsync()
        {
            var libraries = await _libraryService.GetAllAsync();
            LibraryList = libraries
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
                .ToList();
        }
    }
}
