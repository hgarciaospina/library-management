using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Loans
{
    public class DeleteModel : PageModel
    {
        private readonly ILoanService _loanService;
        public DeleteModel(ILoanService loanService) => _loanService = loanService;

        [BindProperty]
        public LoanDto Loan { get; set; } = new LoanDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null) return NotFound();
            Loan = loan;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _loanService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
    }
}
