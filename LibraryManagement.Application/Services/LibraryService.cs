using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IGenericRepository<Library> _repository;
        private readonly IMapper _mapper;

        public LibraryService(IGenericRepository<Library> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LibraryDto>> GetAllAsync()
        {
            var libraries = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<LibraryDto>>(libraries);
        }

        public async Task<LibraryDto> GetByIdAsync(int id)
        {
            var library = await _repository.GetByIdAsync(id);
            return _mapper.Map<LibraryDto>(library);
        }

        public async Task<LibraryDto> CreateAsync(LibraryCreateDto dto)
        {
            var library = _mapper.Map<Library>(dto);
            await _repository.AddAsync(library);
            return _mapper.Map<LibraryDto>(library);
        }

        public async Task UpdateAsync(LibraryUpdateDto dto)
        {
            var library = _mapper.Map<Library>(dto);
            await _repository.UpdateAsync(library);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
