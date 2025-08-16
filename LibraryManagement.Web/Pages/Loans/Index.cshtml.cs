using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Loans
{
    public class IndexModel : PageModel
    {
        private readonly ILoanService _loanService;

        public IndexModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        // List of loans to display in the table
        public IList<LoanDto> Loans { get; set; } = new List<LoanDto>();

        public async Task OnGetAsync()
        {
            // ✅ Use the new service method to include Book and Member navigation properties
            var loans = await _loanService.GetAllWithDetailsAsync();
            Loans = loans.ToList();
        }
    }
}
