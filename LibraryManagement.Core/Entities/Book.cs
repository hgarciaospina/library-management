using System.Collections.Generic;

namespace LibraryManagement.Core.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string ?Title { get; set; }
        public string ?Author { get; set; }
        public string ?ISBN { get; set; }
        public int PublicationYear { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int LibraryId { get; set; }
        public Library ?Library { get; set; }

        public ICollection<Loan> ?Loans { get; set; }
    }
}
