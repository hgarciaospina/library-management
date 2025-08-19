using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Pages.Books
{
    /// <summary>
    /// Page model for deleting a book.
    /// Checks if the book has active loans before deletion.
    /// If there are active loans, deletion is prevented and an error message is shown.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;  // Service to interact with books
        private readonly ILoanService _loanService;  // Service to check for active loans

        /// <summary>
        /// Constructor that injects the required services
        /// </summary>
        /// <param name="bookService">Book service</param>
        /// <param name="loanService">Loan service</param>
        public DeleteModel(IBookService bookService, ILoanService loanService)
        {
            _bookService = bookService;
            _loanService = loanService;
        }

        /// <summary>
        /// Bind property for the book
        /// </summary>
        [BindProperty]
        public BookDto Book { get; set; } = new BookDto();

        /// <summary>
        /// TempData property to store error messages
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// GET method: Retrieves the details of the book to be deleted
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>The page with the book details</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var book = await _bookService.GetByIdAsync(id);  // Get book by ID
            if (book == null) return NotFound();  // If book does not exist, return 404

            Book = book;  // Assign book details to the property
            return Page();  // Return the page with the book loaded
        }

        /// <summary>
        /// POST method: Confirms the deletion of the book.
        /// Checks if the book has active loans before proceeding.
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>Redirects to the same page with error or to Index after successful deletion</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Check if the book has active loans
            var loans = await _loanService.GetLoansByBookIdAsync(id);
            if (loans.Any())  // If there are active loans, prevent deletion
            {
                // Set error message
                ErrorMessage = "Cannot delete the book because it has active loans.";
                return RedirectToPage();  // Reload the page and show the error modal
            }

            // If there are no active loans, proceed with deletion
            await _bookService.DeleteAsync(id);
            return RedirectToPage("Index");  // Redirect to the books list after deletion
        }
    }
}
