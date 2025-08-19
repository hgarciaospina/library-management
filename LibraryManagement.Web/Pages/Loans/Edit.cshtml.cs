using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation;
using FluentValidation.Results;

namespace LibraryManagement.Web.Pages.Loans
{
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
        /// Carga los datos del préstamo y rellena los dropdowns.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loanDto = await _loanService.GetByIdWithDetailsAsync(id);
            if (loanDto == null) return NotFound();

            // Mapear LoanDto → LoanUpdateDto
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
        /// Maneja la sumisión del formulario para actualizar el préstamo.
        /// Valida con FluentValidation y recarga dropdowns si hay errores.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Recargar LoanDate desde la base de datos antes de la validación
            var loanFromDb = await _loanService.GetByIdWithDetailsAsync(Loan.Id);
            if (loanFromDb == null) return NotFound();

            Loan.LoanDate = loanFromDb.LoanDate;

            // Validar con FluentValidation
            ValidationResult result = _validator.Validate(Loan);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    // Asociar cada error a ModelState para que se muestre en la vista
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                // Recargar dropdowns para mantener selecciones previas
                await OnGetAsync(Loan.Id);

                return Page();
            }

            // Actualizar préstamo y disponibilidad del libro
            await _loanService.UpdateAsync(Loan);

            // Redirigir de nuevo a la lista de préstamos
            return RedirectToPage("Index", new { libraryId = Loan.LibraryId });
        }
    }
}
