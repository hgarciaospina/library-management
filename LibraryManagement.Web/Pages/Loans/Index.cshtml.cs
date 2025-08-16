using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for displaying a list of Loans.
    /// Supports filtering by LibraryId passed as query parameter.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILoanService _loanService;

        public IndexModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// List of loans to display in the table.
        /// Includes BookTitle and MemberFullName.
        /// </summary>
        public IList<LoanDto> Loans { get; set; } = new List<LoanDto>();

        /// <summary>
        /// Selected LibraryId from the dropdown menu.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int? LibraryId { get; set; }

        /// <summary>
        /// OnGetAsync loads loans with related Book and Member details.
        /// Filters by LibraryId if provided.
        /// </summary>
        public async Task OnGetAsync()
        {
            // Fetch all loans including navigation properties
            var allLoans = await _loanService.GetAllWithDetailsAsync();

            // Filter by selected library if LibraryId is set
            if (LibraryId.HasValue)
            {
                Loans = allLoans
                    .Where(l => l.LibraryId == LibraryId.Value)
                    .ToList();
            }
            else
            {
                Loans = allLoans.ToList();
            }
        }
    }
}
