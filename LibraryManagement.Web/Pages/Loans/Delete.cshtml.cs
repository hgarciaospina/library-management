// Delete.cshtml.cs
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for deleting a Loan.
    /// Uses LoanDetailsDto to display full loan information before confirmation.
    /// Redirects to the Index page of the same library after deletion.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly ILoanService _loanService;

        /// <summary>
        /// Constructor injects the Loan service dependency.
        /// </summary>
        public DeleteModel(ILoanService loanService) => _loanService = loanService;

        /// <summary>
        /// Loan property bound to the view. Uses LoanDetailsDto for full loan info.
        /// </summary>
        [BindProperty]
        public LoanDetailsDto Loan { get; set; } = new LoanDetailsDto();

        /// <summary>
        /// GET handler retrieves loan details by ID.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loan = await _loanService.GetDetailsByIdAsync(id);
            if (loan == null) return NotFound();

            Loan = loan;
            return Page();
        }

        /// <summary>
        /// POST handler deletes the loan and redirects to Index of the same library.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            await _loanService.DeleteAsync(Loan.Id);
            return RedirectToPage("/Loans/Index", new { libraryId = Loan.LibraryId });
        }
    }
}
