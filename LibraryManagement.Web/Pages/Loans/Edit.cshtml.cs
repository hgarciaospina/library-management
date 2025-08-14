using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Pages.Loans
{
    public class EditModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;

        public EditModel(ILoanService loanService, IBookService bookService, IMemberService memberService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _memberService = memberService;
        }

        [BindProperty]
        public LoanUpdateDto Loan { get; set; } = new LoanUpdateDto();

        public IEnumerable<SelectListItem> Books { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null) return NotFound();

            Loan = new LoanUpdateDto
            {
                Id = loan.Id,
                BookId = loan.BookId,
                MemberId = loan.MemberId,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate
            };

            Books = (await _bookService.GetAllAsync())
                    .Select(b => new SelectListItem(b.Title, b.Id.ToString()));

            Members = (await _memberService.GetAllAsync())
                      .Select(m => new SelectListItem($"{m.FirstName} {m.LastName}", m.Id.ToString()));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(Loan.Id); // recargar listas
                return Page();
            }

            await _loanService.UpdateAsync(Loan);
            return RedirectToPage("Index");
        }
    }
}
