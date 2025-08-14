using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberDto>> GetAllAsync();
        Task<MemberDto> GetByIdAsync(int id);
        Task<MemberDto> CreateAsync(MemberCreateDto dto);
        Task UpdateAsync(MemberUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
