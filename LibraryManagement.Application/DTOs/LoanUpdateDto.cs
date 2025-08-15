namespace LibraryManagement.Application.DTOs
{
    public class LoanUpdateDto
    {
        public int Id { get; set; }
        public int LibraryId { get; set; } // Added for consistency with entity Loan
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
