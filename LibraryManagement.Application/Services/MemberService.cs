using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validations;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    /// <summary>
    /// Service implementation for Member entity.
    /// Handles CRUD operations and validation safely for Web and API.
    /// </summary>
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _repository;
        private readonly IMapper _mapper;
        private readonly ILibraryService _libraryService;
        private readonly IValidator<MemberCreateDto> _syncValidator; // Synchronous validator for Razor Pages / MVC

        public MemberService(
            IGenericRepository<Member> repository,
            IMapper mapper,
            ILibraryService libraryService,
            IValidator<MemberCreateDto> syncValidator) // Inject sync validator
        {
            _repository = repository;
            _mapper = mapper;
            _libraryService = libraryService;
            _syncValidator = syncValidator;
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
        /// Uses synchronous validation for Web (Razor Pages) and manual async validation for Library existence.
        /// </summary>
        public async Task<MemberDto> CreateAsync(MemberCreateDto dto)
        {
            // 1️⃣ Validación síncrona de campos obligatorios
            var validationResult = _syncValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // 2️⃣ Validación async manual: verificar que LibraryId exista
            var library = await _libraryService.GetByIdAsync(dto.LibraryId);
            if (library == null)
                throw new ValidationException("The selected library does not exist.");

            // 3️⃣ Mapear y guardar
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
