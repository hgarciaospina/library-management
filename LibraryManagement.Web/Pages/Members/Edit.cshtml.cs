using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly IMemberService _memberService;
        public EditModel(IMemberService memberService) => _memberService = memberService;

        [BindProperty]
        public MemberUpdateDto Member { get; set; } = new MemberUpdateDto();

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
                PhoneNumber = member.PhoneNumber
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _memberService.UpdateAsync(Member);
            return RedirectToPage("Index");
        }
    }
}
