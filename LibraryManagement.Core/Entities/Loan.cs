using System;

namespace LibraryManagement.Core.Entities
{
    public class Loan
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int LibraryId { get; set; }
        public Library Library { get; set; }

        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
    }
}
