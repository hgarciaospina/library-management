using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Loans
{
    public class DetailsModel : PageModel
    {
        private readonly ILoanService _loanService;
        public DetailsModel(ILoanService loanService) => _loanService = loanService;

        [BindProperty]
        public LoanDto Loan { get; set; } = new LoanDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null) return NotFound();
            Loan = loan;
            return Page();
        }
    }
}
