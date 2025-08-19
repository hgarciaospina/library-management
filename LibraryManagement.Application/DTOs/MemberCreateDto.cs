// MemberCreateDto.cs
namespace LibraryManagement.Application.DTOs
{
    public class MemberCreateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        //public int? LibraryId { get; set; } // <-- now nullable
        public int LibraryId { get; set; }
    }
}
