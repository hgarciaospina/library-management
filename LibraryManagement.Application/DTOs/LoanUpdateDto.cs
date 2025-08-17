namespace LibraryManagement.Application.DTOs
{
    public class LoanUpdateDto
    {
        public int Id { get; set; }

        public int LibraryId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }

        // Added to validate ReturnDate
        public DateTime LoanDate
        {
            get; set;

        }
    }
}
