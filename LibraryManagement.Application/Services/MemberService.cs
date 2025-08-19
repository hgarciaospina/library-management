using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using FluentValidation;
using LibraryManagement.Application.Validations.Api;

namespace LibraryManagement.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _repository;
        private readonly IMapper _mapper;
        private readonly MemberCreateDtoApiValidator _validator; // Inject the API validator

        // Constructor now includes the API validator
        public MemberService(IGenericRepository<Member> repository, IMapper mapper, MemberCreateDtoApiValidator validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
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

        // Modify the CreateAsync method to include validation
        public async Task<MemberDto> CreateAsync(MemberCreateDto dto)
        {
            // Validate using the injected API validator
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                // You can throw an exception or handle validation errors here
                throw new ValidationException(validationResult.Errors);
            }

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
