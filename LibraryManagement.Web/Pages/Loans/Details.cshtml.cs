using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for displaying full details of a loan.
    /// Uses LoanDetailsDto to show member, book, library, and dates.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly ILoanService _loanService;

        /// <summary>
        /// Constructor injecting the loan service.
        /// </summary>
        public DetailsModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Bound property representing the loan with full details.
        /// </summary>
        [BindProperty]
        public LoanDetailsDto Loan { get; set; } = new LoanDetailsDto();

        /// <summary>
        /// Handles GET request to display loan details.
        /// </summary>
        /// <param name="id">The loan ID.</param>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loan = await _loanService.GetDetailsByIdAsync(id);
            if (loan == null)
                return NotFound();

            Loan = loan;
            return Page();
        }
    }
}
