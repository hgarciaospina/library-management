using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Members
{
    public class DeleteModel : PageModel
    {
        private readonly IMemberService _memberService;
        public DeleteModel(IMemberService memberService) => _memberService = memberService;

        [BindProperty]
        public MemberDto Member { get; set; } = new MemberDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null) return NotFound();
            Member = member;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _memberService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
    }
}
