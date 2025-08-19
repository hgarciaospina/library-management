using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validations;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LibraryManagement.Infrastructure.Data;

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
        private readonly LibraryContext _context; // Add DbContext for tracking management

        public MemberService(
            IGenericRepository<Member> repository,
            IMapper mapper,
            ILibraryService libraryService,
            IValidator<MemberCreateDto> syncValidator,
            LibraryContext context) // Inject DbContext
        {
            _repository = repository;
            _mapper = mapper;
            _libraryService = libraryService;
            _syncValidator = syncValidator;
            _context = context;
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
            // SOLUCIÓN AL ERROR DE TRACKING: Usar enfoque seguro
            await UpdateMemberSafelyAsync(dto);
        }

        /// <summary>
        /// Safe update method that prevents entity tracking conflicts
        /// </summary>
        private async Task UpdateMemberSafelyAsync(MemberUpdateDto dto)
        {
            // PRIMERO: Detach any entity with the same ID that might be tracked
            var existingTrackedEntity = _context.ChangeTracker.Entries<Member>()
                .FirstOrDefault(e => e.Entity.Id == dto.Id);

            if (existingTrackedEntity != null)
            {
                existingTrackedEntity.State = EntityState.Detached;
            }

            // SEGUNDO: Get the member from database without tracking
            var member = await _context.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.Id);

            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {dto.Id} not found.");
            }

            // TERCERO: Map and update only changed properties
            var updatedMember = _mapper.Map<Member>(dto);

            // CUARTO: Attach and mark as modified
            _context.Members.Attach(updatedMember);
            _context.Entry(updatedMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new KeyNotFoundException($"Member with ID {dto.Id} no longer exists.");
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the member.", ex);
            }
        }

        /// <summary>
        /// Alternative update method with explicit property setting
        /// </summary>
        private async Task UpdateMemberExplicitlyAsync(MemberUpdateDto dto)
        {
            // Get member with tracking
            var member = await _repository.GetByIdAsync(dto.Id);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {dto.Id} not found.");
            }

            // Update only the properties that are provided (not null)
            if (!string.IsNullOrEmpty(dto.FirstName))
                member.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                member.LastName = dto.LastName;

            if (!string.IsNullOrEmpty(dto.Email))
                member.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                member.PhoneNumber = dto.PhoneNumber;

            if (dto.LibraryId.HasValue)
                member.LibraryId = dto.LibraryId.Value;

            // Use repository update
            await _repository.UpdateAsync(member);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Additional method to get members by library
        /// </summary>
        public async Task<IEnumerable<MemberDto>> GetByLibraryIdAsync(int libraryId)
        {
            var members = await _context.Members
                .Where(m => m.LibraryId == libraryId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<MemberDto>>(members);
        }

        /// <summary>
        /// Method to check if email already exists
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email, int? excludeMemberId = null)
        {
            var query = _context.Members.AsNoTracking().Where(m => m.Email == email);

            if (excludeMemberId.HasValue)
            {
                query = query.Where(m => m.Id != excludeMemberId.Value);
            }

            return await query.AnyAsync();
        }
    }
}