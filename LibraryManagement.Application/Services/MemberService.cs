using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;

namespace LibraryManagement.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _repository;
        private readonly IMapper _mapper;

        public MemberService(IGenericRepository<Member> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MemberDto>> GetAllAsync()
        {
            var members = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<MemberDto>>(members);
        }

        public async Task<MemberDto> GetByIdAsync(int id)
        {
            var member = await _repository.GetByIdAsync(id);
            return _mapper.Map<MemberDto>(member);
        }

        public async Task<MemberDto> CreateAsync(MemberCreateDto dto)
        {
            var member = _mapper.Map<Member>(dto);
            await _repository.AddAsync(member);
            return _mapper.Map<MemberDto>(member);
        }

        public async Task UpdateAsync(MemberUpdateDto dto)
        {
            var member = _mapper.Map<Member>(dto);
            await _repository.UpdateAsync(member);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
