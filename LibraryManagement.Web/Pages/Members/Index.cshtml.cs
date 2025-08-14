using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Members
{
    public class IndexModel : PageModel
    {
        private readonly IMemberService _memberService;
        public IndexModel(IMemberService memberService) => _memberService = memberService;

        public IList<MemberDto> Members { get; set; } = new List<MemberDto>();

        public async Task OnGetAsync()
        {
            Members = (await _memberService.GetAllAsync()).ToList();
        }
    }
}
