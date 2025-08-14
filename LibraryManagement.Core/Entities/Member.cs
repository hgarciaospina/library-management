using System;
using System.Collections.Generic;

namespace LibraryManagement.Core.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public int LibraryId { get; set; }
        public Library Library { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
