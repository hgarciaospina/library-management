using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly IMemberService _memberService;
        public CreateModel(IMemberService memberService) => _memberService = memberService;

        [BindProperty]
        public MemberCreateDto Member { get; set; } = new MemberCreateDto();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _memberService.CreateAsync(Member);
            return RedirectToPage("Index");
        }
    }
}
