
namespace LibraryManagement.Application.DTOs
{
    public class LoanCreateDto
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
    }
}
