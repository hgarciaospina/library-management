namespace LibraryManagement.Application.DTOs
{
    /// <summary>
    /// DTO for displaying full loan details.
    /// Includes all fields needed for the Details view and full listings.
    /// </summary>
    public class LoanDetailsDto
    {
        // Unique identifier of the loan
        public int Id { get; set; }

        // -----------------------------
        // Library information
        // -----------------------------
        public int LibraryId { get; set; }
        public string LibraryName { get; set; } = string.Empty;

        // -----------------------------
        // Member information
        // -----------------------------
        public int MemberId { get; set; }
        public string MemberFullName { get; set; } = string.Empty;

        // -----------------------------
        // Book information
        // -----------------------------
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;

        // Corrected property name to fix AutoMapper error
        public string BookISBN { get; set; } = string.Empty;

        // -----------------------------
        // Loan dates
        // -----------------------------
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
