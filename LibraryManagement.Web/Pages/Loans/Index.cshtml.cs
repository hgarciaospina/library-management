using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Loans
{
    public class IndexModel : PageModel
    {
        private readonly ILoanService _loanService;

        public IndexModel(ILoanService loanService) => _loanService = loanService;

        public IList<LoanDto> Loans { get; set; } = new List<LoanDto>();

        public async Task OnGetAsync()
        {
            // Convertimos IEnumerable a IList
            Loans = (await _loanService.GetAllAsync()).ToList();
        }
    }
}
