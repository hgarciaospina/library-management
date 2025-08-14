using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Pages.Loans
{
    public class CreateModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;

        public CreateModel(ILoanService loanService, IBookService bookService, IMemberService memberService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _memberService = memberService;
        }

        [BindProperty]
        public LoanCreateDto Loan { get; set; } = new LoanCreateDto();

        public IEnumerable<SelectListItem> Books { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Llenar listas desplegables
            Books = (await _bookService.GetAllAsync())
                    .Where(b => b.IsAvailable)
                    .Select(b => new SelectListItem(b.Title, b.Id.ToString()));

            Members = (await _memberService.GetAllAsync())
                      .Select(m => new SelectListItem($"{m.FirstName} {m.LastName}", m.Id.ToString()));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Volver a cargar las listas si hay error
                return Page();
            }

            await _loanService.CreateAsync(Loan);
            return RedirectToPage("Index");
        }
    }
}
