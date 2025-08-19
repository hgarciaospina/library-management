// Pages/Members/Delete.cshtml.cs
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Members
{
    /// <summary>
    /// Page model for deleting a member.
    /// Checks if the member has active loans before deletion.
    /// If there are active loans, deletion is prevented and an error message is shown.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;

        /// <summary>
        /// Constructor injecting member and loan services.
        /// </summary>
        public DeleteModel(IMemberService memberService, ILoanService loanService)
        {
            _memberService = memberService;
            _loanService = loanService;
        }

        /// <summary>
        /// The member to display in the Delete view.
        /// </summary>
        [BindProperty]
        public MemberDto Member { get; set; } = new MemberDto();

        /// <summary>
        /// TempData property for storing error messages.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// GET: Loads the member details for confirmation.
        /// </summary>
        /// <param name="id">Member ID.</param>
        /// <returns>The page with member details.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null) return NotFound();

            Member = member;
            return Page();
        }

        /// <summary>
        /// POST: Confirms deletion of the member.
        /// Checks for active loans before deleting.
        /// </summary>
        /// <param name="id">Member ID.</param>
        /// <returns>Redirects to Index or reloads page with error message.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var loans = await _loanService.GetLoansByMemberIdAsync(id);
            if (loans.Any())
            {
                ErrorMessage = "Cannot delete the member because they have active loans.";
                return RedirectToPage();
            }

            await _memberService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
    }
}
