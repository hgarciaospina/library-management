using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Repositories;

namespace LibraryManagement.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly IGenericRepository<Loan> _repository;
        private readonly IMapper _mapper;

        public LoanService(IGenericRepository<Loan> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        // ✅ New: Get all loans including Book and Member
        public async Task<IEnumerable<LoanDto>> GetAllWithDetailsAsync()
        {
            // Use repository method that includes navigation properties
            var loans = await _repository.GetAllIncludingAsync(l => l.Book, l => l.Member);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _repository.GetByIdAsync(id);
            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> CreateAsync(LoanCreateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);
            await _repository.AddAsync(loan);
            return _mapper.Map<LoanDto>(loan);
        }

        public async Task UpdateAsync(LoanUpdateDto dto)
        {
            var loan = _mapper.Map<Loan>(dto);
            await _repository.UpdateAsync(loan);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
