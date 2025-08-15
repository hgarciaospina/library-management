namespace LibraryManagement.Application.DTOs
{
    public class LoanCreateDto
    {
        public int LibraryId { get; set; } // Needed to link the loan to a library
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
    }
}
