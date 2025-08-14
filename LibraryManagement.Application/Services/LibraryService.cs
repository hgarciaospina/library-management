using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public LibraryService(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LibraryDto>> GetAllAsync()
        {
            var libraries = await _context.Libraries.ToListAsync();
            return _mapper.Map<IEnumerable<LibraryDto>>(libraries);
        }

        public async Task<LibraryDto> GetByIdAsync(int id)
        {
            var library = await _context.Libraries.FindAsync(id);
            return _mapper.Map<LibraryDto>(library);
        }

        // ⚡ Implementación correcta de AddAsync
        public async Task<LibraryDto> CreateAsync(LibraryCreateDto libraryCreateDto)
        {
            var library = _mapper.Map<Library>(libraryCreateDto);
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();
            return _mapper.Map<LibraryDto>(library);
        }

        public async Task UpdateAsync(LibraryUpdateDto libraryUpdateDto)
        {
            var library = await _context.Libraries.FindAsync(libraryUpdateDto.Id);
            if (library == null) throw new KeyNotFoundException("Library not found");

            _mapper.Map(libraryUpdateDto, library);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var library = await _context.Libraries.FindAsync(id);
            if (library == null) throw new KeyNotFoundException("Library not found");

            _context.Libraries.Remove(library);
            await _context.SaveChangesAsync();
        }
    }
}
