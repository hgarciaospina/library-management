using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Books
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<BookCreateDto, Book>();
            CreateMap<BookUpdateDto, Book>();

            // Members
            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<MemberCreateDto, Member>();
            CreateMap<MemberUpdateDto, Member>();

            // Loans
            CreateMap<Loan, LoanDto>().ReverseMap();
            CreateMap<LoanCreateDto, Loan>();
            CreateMap<LoanUpdateDto, Loan>();

            // Libraries
            CreateMap<Library, LibraryDto>().ReverseMap();
            CreateMap<LibraryCreateDto, Library>();
            CreateMap<LibraryUpdateDto, Library>();
        }
    }
}
