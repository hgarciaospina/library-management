namespace LibraryManagement.Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }                  // Para GET, PUT, DELETE
        public string ?Title { get; set; }
        public string ?Author { get; set; }
        public string ?ISBN { get; set; }
        public int PublicationYear { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int LibraryId { get; set; }           // Relación con Library
    }

}
