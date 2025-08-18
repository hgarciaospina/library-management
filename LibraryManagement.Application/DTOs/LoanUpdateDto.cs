using System;

namespace LibraryManagement.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating a Loan.
    /// Contains fields that can be updated: Book, DueDate, ReturnDate.
    /// LoanDate is included for validation purposes (ReturnDate cannot be before LoanDate).
    /// </summary>
    public class LoanUpdateDto
    {
        public int Id { get; set; }

        public int LibraryId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }

        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }

        // Used for validation: ReturnDate must be >= LoanDate and <= Today
        public DateTime LoanDate { get; set; }
    }
}
