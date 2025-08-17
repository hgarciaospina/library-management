namespace LibraryManagement.Application.DTOs
{
    public class MemberUpdateDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? LibraryId { get; set; }
    }
}
