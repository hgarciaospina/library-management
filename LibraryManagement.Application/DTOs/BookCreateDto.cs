namespace LibraryManagement.Application.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;

        // Cambiar a int? para evitar parseos manuales
        public int? PublicationYear { get; set; }

        public int? LibraryId { get; set; }
    }
}
