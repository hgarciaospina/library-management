// BookCreateDto.cs
namespace LibraryManagement.Application.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public int? LibraryId { get; set; }  // <-- nullable
        public bool IsAvailable { get; set; } = true;
    }
}