
namespace LibraryManagement.Core.Entities
{
    public class Library
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
