using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Loans
{
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

        [BindProperty]
        public LoanCreateDto Loan { get; set; } = new LoanCreateDto();

        public IEnumerable<SelectListItem> Libraries { get; set; } = new List<SelectListItem>();

        // Load libraries for first dropdown
        public async Task<IActionResult> OnGetAsync()
        {
            Libraries = (await _libraryService.GetAllAsync())
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // reload libraries if validation fails
                return Page();
            }

            // Create loan
            await _loanService.CreateAsync(Loan);

            // --- CORRECTED: Mark the book as unavailable without EF tracking error ---
            var trackedBook = await _bookService.GetEntityByIdAsync(Loan.BookId); // returns tracked Book entity
            if (trackedBook != null)
            {
                trackedBook.IsAvailable = false; // modify directly
                await _bookService.SaveEntityAsync(trackedBook); // save changes
            }

            return RedirectToPage("Index");
        }

        // Handler: get books by library for AJAX
        public async Task<JsonResult> OnGetBooksByLibrary(int libraryId)
        {
            var books = (await _bookService.GetAllAsync())
                .Where(b => b.LibraryId == libraryId && b.IsAvailable)
                .Select(b => new { value = b.Id, text = b.Title });

            return new JsonResult(books);
        }

        // Handler: get members by library for AJAX
        public async Task<JsonResult> OnGetMembersByLibrary(int libraryId)
        {
            var members = (await _memberService.GetAllAsync())
                .Where(m => m.LibraryId == libraryId)
                .Select(m => new { value = m.Id, text = $"{m.FirstName} {m.LastName}" });

            return new JsonResult(members);
        }
    }
}
