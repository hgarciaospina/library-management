namespace LibraryManagement.Application.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int LibraryId { get; set; } // Added for UI display
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string MemberFullName { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }

        // Library name comes from the related entity
        public string LibraryName { get; set; } = string.Empty;

        // Optional: include navigation for service mapping
        public BookDto? Book { get; set; }
        public MemberDto? Member { get; set; }
    }
}
