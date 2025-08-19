using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using FluentValidation;
using LibraryManagement.Application.Validations; // ✅ usar solo el validator síncrono aquí

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service implementation for the Member entity.
    /// Handles CRUD operations and validation for both Web and API safely.
    /// </summary>
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _repository;
        private readonly IMapper _mapper;
        private readonly MemberCreateDtoValidator _validator; // Validator síncrono

        /// <summary>
        /// Constructor injects repository, mapper, and synchronous validator.
        /// Async rules should only be called manually inside service if needed.
        /// </summary>
        public MemberService(
            IGenericRepository<Member> repository,
            IMapper mapper,
            MemberCreateDtoValidator validator) // ✅ Inyectar solo el validator síncrono
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

        /// <summary>
        /// Creates a new member.
        /// Uses synchronous validator for automatic ASP.NET validation.
        /// If API async rules are needed, call them manually here.
        /// </summary>
        public async Task<MemberDto> CreateAsync(MemberCreateDto dto)
        {
            // Validación síncrona segura para Web
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
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
