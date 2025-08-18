using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation;
using FluentValidation.Results;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for editing a Loan.
    /// Only Book, DueDate, and ReturnDate can be modified.
    /// Library and Member are read-only.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly IValidator<LoanUpdateDto> _validator;

        public EditModel(
            ILoanService loanService,
            IBookService bookService,
            IMemberService memberService,
            IValidator<LoanUpdateDto> validator)
        {
            _loanService = loanService;
            _bookService = bookService;
            _memberService = memberService;
            _validator = validator;
        }

        [BindProperty]
        public LoanUpdateDto Loan { get; set; } = new LoanUpdateDto();

        public IEnumerable<SelectListItem> Books { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();
        public string LibraryName { get; set; } = string.Empty;

        /// <summary>
        /// Load loan data and populate dropdowns.
        /// Maps LoanDto → LoanUpdateDto to avoid type mismatch.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loanDto = await _loanService.GetByIdWithDetailsAsync(id);
            if (loanDto == null) return NotFound();

            // Map LoanDto → LoanUpdateDto including LoanDate
            Loan = new LoanUpdateDto
            {
                Id = loanDto.Id,
                LibraryId = loanDto.LibraryId,
                MemberId = loanDto.MemberId,
                BookId = loanDto.BookId,
                DueDate = loanDto.DueDate,
                ReturnDate = loanDto.ReturnDate,
                LoanDate = loanDto.LoanDate
            };

            LibraryName = loanDto.LibraryName;

            Books = (await _bookService.GetAllAsync())
                .Select(b => new SelectListItem(b.Title, b.Id.ToString(), b.Id == Loan.BookId));

            Members = new List<SelectListItem>
            {
                new SelectListItem($"{loanDto.MemberFullName}", loanDto.MemberId.ToString(), true)
            };

            return Page();
        }

        /// <summary>
        /// Handle form submission to update loan.
        /// Loads LoanDate before validating to ensure ReturnDate is validated correctly.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Reload LoanDate from database before validation
            var loanFromDb = await _loanService.GetByIdWithDetailsAsync(Loan.Id);
            if (loanFromDb == null) return NotFound();

            Loan.LoanDate = loanFromDb.LoanDate;

            // Validate using FluentValidation
            ValidationResult result = _validator.Validate(Loan);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                // Reload dropdowns
                await OnGetAsync(Loan.Id);
                return Page();
            }

            // Update loan and book availability
            await _loanService.UpdateAsync(Loan);

            // Redirect back to loans index for the library
            return RedirectToPage("Index", new { libraryId = Loan.LibraryId });
        }
    }
}
