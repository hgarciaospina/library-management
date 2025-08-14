using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly IMemberService _memberService;
        public DetailsModel(IMemberService memberService) => _memberService = memberService;

        [BindProperty]
        public MemberDto Member { get; set; } = new MemberDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null) return NotFound();
            Member = member;
            return Page();
        }
    }
}
