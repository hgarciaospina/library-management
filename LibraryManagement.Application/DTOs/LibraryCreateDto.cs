namespace LibraryManagement.Application.DTOs
{
    public class LibraryCreateDto
    {
        public string Name { get; set; } = string.Empty;       // inicializa para evitar null
        public string Address { get; set; } = string.Empty;    // inicializa para evitar null
    }
}
