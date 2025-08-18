using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for displaying a list of Loans.
    /// Supports filtering by LibraryId passed as query parameter.
    /// Shows the selected library name even if there are no loans.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly ILibraryService _libraryService;

        public IndexModel(ILoanService loanService, ILibraryService libraryService)
        {
            _loanService = loanService;
            _libraryService = libraryService;
        }

        /// <summary>
        /// List of loans to display in the table.
        /// </summary>
        public IList<LoanDto> Loans { get; set; } = new List<LoanDto>();

        /// <summary>
        /// Selected LibraryId from the dropdown or query string.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int? LibraryId { get; set; }

        /// <summary>
        /// Selected library name to display in header.
        /// </summary>
        public string? SelectedLibraryName { get; set; }

        /// <summary>
        /// OnGetAsync loads loans with related Book and Member details.
        /// Filters by LibraryId if provided and sets SelectedLibraryName.
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

                // Retrieve library name even if no loans exist
                var library = await _libraryService.GetByIdAsync(LibraryId.Value);
                if (library != null)
                {
                    SelectedLibraryName = library.Name;
                }
            }
            else
            {
                Loans = allLoans.ToList();
                SelectedLibraryName = null;
            }
        }
    }
}
