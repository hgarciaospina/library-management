// BookUpdateDto.cs
namespace LibraryManagement.Application.DTOs
{
    public class BookUpdateDto : BookCreateDto
    {
        public int Id { get; set; }
    }
}