using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Loans
{
    /// <summary>
    /// PageModel for creating a new Loan.
    /// This page only handles UI interactions.
    /// All business logic, including marking books as unavailable, is handled in LoanService.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService; // still needed for AJAX dropdowns
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
        /// DTO bound to the form for creating a loan.
        /// </summary>
        [BindProperty]
        public LoanCreateDto Loan { get; set; } = new LoanCreateDto();

        /// <summary>
        /// Dropdown list of libraries for the UI.
        /// </summary>
        public IEnumerable<SelectListItem> Libraries { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Loads libraries for the dropdown when the page is displayed.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            Libraries = (await _libraryService.GetAllAsync())
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()));

            return Page();
        }

        /// <summary>
        /// Handles form submission to create a new loan.
        /// Business logic like marking the book as unavailable is handled in LoanService.
        /// Redirects to Index page filtered by the LibraryId of the created loan.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload libraries if validation fails
                await OnGetAsync();
                return Page();
            }

            // Create the loan using the service
            // Service ensures book is marked unavailable and all relations are handled
            var createdLoan = await _loanService.CreateAsync(Loan);

            // Redirect to Index filtered by the correct LibraryId
            return RedirectToPage("Index", new { libraryId = createdLoan.LibraryId });
        }

        /// <summary>
        /// AJAX endpoint to get available books filtered by library.
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
        /// AJAX endpoint to get members filtered by library.
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
