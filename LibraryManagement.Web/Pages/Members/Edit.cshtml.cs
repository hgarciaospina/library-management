using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly ILibraryService _libraryService;

        public EditModel(IMemberService memberService, ILibraryService libraryService)
        {
            _memberService = memberService;
            _libraryService = libraryService;
        }

        [BindProperty]
        public MemberUpdateDto Member { get; set; } = new MemberUpdateDto();

        public List<SelectListItem> LibraryList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null) return NotFound();

            Member = new MemberUpdateDto
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                LibraryId = member.LibraryId
            };

            await LoadLibrariesAsync(Member.LibraryId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLibrariesAsync(Member.LibraryId);
                return Page();
            }

            await _memberService.UpdateAsync(Member);
            return RedirectToPage("Index");
        }

        private async Task LoadLibrariesAsync(int? selectedLibraryId = null)
        {
            var libraries = await _libraryService.GetAllAsync();
            LibraryList = libraries
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name,
                    Selected = (selectedLibraryId.HasValue && l.Id == selectedLibraryId.Value)
                })
                .ToList();
        }
    }
}
