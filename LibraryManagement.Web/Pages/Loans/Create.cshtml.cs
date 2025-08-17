using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FluentValidation; // Required for IValidator
using FluentValidation.Results;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for creating a loan.
    /// Ensures library comes from query and is read-only.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly ILibraryService _libraryService;
        private readonly IMapper _mapper;
        private readonly IValidator<LoanCreateDto> _validator; // Loan validator

        public CreateModel(
            ILoanService loanService,
            IBookService bookService,
            IMemberService memberService,
            ILibraryService libraryService,
            IMapper mapper,
            IValidator<LoanCreateDto> validator) // Inject LoanValidator
        {
            _loanService = loanService;
            _bookService = bookService;
            _memberService = memberService;
            _libraryService = libraryService;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// DTO bound to the form.
        /// </summary>
        [BindProperty]
        public LoanCreateDto Loan { get; set; } = new LoanCreateDto();

        /// <summary>
        /// Displayed library name.
        /// </summary>
        public string SelectedLibraryName { get; set; } = string.Empty;

        /// <summary>
        /// Books for selected library.
        /// </summary>
        public List<(int Id, string Title)> Books { get; set; } = new();

        /// <summary>
        /// Members for selected library.
        /// </summary>
        public List<(int Id, string FullName)> Members { get; set; } = new();

        /// <summary>
        /// Load page with optional libraryId from query.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? libraryId)
        {
            if (!libraryId.HasValue)
            {
                return BadRequest("LibraryId is required in query.");
            }

            Loan.LibraryId = libraryId.Value;

            var library = (await _libraryService.GetAllAsync())
                .FirstOrDefault(l => l.Id == libraryId.Value);

            if (library == null)
            {
                return NotFound("Library not found.");
            }

            SelectedLibraryName = library.Name;

            // Load books and members for this library
            Books = (await _bookService.GetAllAsync())
                .Where(b => b.LibraryId == libraryId.Value && b.IsAvailable)
                .Select(b => (b.Id, b.Title))
                .ToList();

            Members = (await _memberService.GetAllAsync())
                .Where(m => m.LibraryId == libraryId.Value)
                .Select(m => (m.Id, $"{m.FirstName} {m.LastName}"))
                .ToList();

            return Page();
        }

        /// <summary>
        /// Handles form submission to create a loan.
        /// LibraryId comes from query/hidden input.
        /// Includes FluentValidation validation.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Run validation with FluentValidation
            ValidationResult validationResult = _validator.Validate(Loan);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                await OnGetAsync(Loan.LibraryId);
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync(Loan.LibraryId);
                return Page();
            }

            var createdLoan = await _loanService.CreateAsync(Loan);

            return RedirectToPage("Index", new { libraryId = createdLoan.LibraryId });
        }
    }
}
