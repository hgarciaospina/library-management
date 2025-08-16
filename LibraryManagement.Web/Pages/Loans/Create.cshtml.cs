using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for creating a new Loan.
    /// Supports AJAX calls to get books and members filtered by library.
    /// After creation, redirects to Index filtered by the selected LibraryId.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly ILibraryService _libraryService;
        private readonly IMapper _mapper;

        public CreateModel(
            ILoanService loanService,
            IBookService bookService,
            IMemberService memberService,
            ILibraryService libraryService,
            IMapper mapper
        )
        {
            _loanService = loanService;
            _bookService = bookService;
            _memberService = memberService;
            _libraryService = libraryService;
            _mapper = mapper;
        }

        /// <summary>
        /// Loan DTO bound to the form.
        /// </summary>
        [BindProperty]
        public LoanCreateDto Loan { get; set; } = new LoanCreateDto();

        /// <summary>
        /// Libraries dropdown list items.
        /// </summary>
        public IEnumerable<SelectListItem> Libraries { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// OnGetAsync loads libraries for the dropdown when the page is first displayed.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            Libraries = (await _libraryService.GetAllAsync())
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()));

            return Page();
        }

        /// <summary>
        /// OnPostAsync handles form submission to create a new loan.
        /// After creation, redirects to Index with LibraryId to filter the loans table.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload libraries if validation fails
                await OnGetAsync();
                return Page();
            }

            // Create the loan
            var createdLoan = await _loanService.CreateAsync(Loan);

            // --- Ensure LibraryId is loaded correctly from related Book ---
            var loanWithLibrary = await _loanService.GetByIdWithDetailsAsync(createdLoan.Id);

            // Mark the book as unavailable
            var trackedBook = await _bookService.GetEntityByIdAsync(Loan.BookId);
            if (trackedBook != null)
            {
                trackedBook.IsAvailable = false; // modify directly
                await _bookService.SaveEntityAsync(trackedBook); // save changes
            }

            // Redirect to Index filtered by the correct LibraryId
            return RedirectToPage("Index", new { libraryId = loanWithLibrary.LibraryId });
        }

        /// <summary>
        /// AJAX handler to get available books filtered by library.
        /// Returns JSON { value, text } for populating dropdowns.
        /// </summary>
        public async Task<JsonResult> OnGetBooksByLibrary(int libraryId)
        {
            var books = (await _bookService.GetAllAsync())
                .Where(b => b.LibraryId == libraryId && b.IsAvailable)
                .Select(b => new { value = b.Id, text = b.Title });

            return new JsonResult(books);
        }

        /// <summary>
        /// AJAX handler to get members filtered by library.
        /// Returns JSON { value, text } for populating dropdowns.
        /// </summary>
        public async Task<JsonResult> OnGetMembersByLibrary(int libraryId)
        {
            var members = (await _memberService.GetAllAsync())
                .Where(m => m.LibraryId == libraryId)
                .Select(m => new { value = m.Id, text = $"{m.FirstName} {m.LastName}" });

            return new JsonResult(members);
        }
    }
}
