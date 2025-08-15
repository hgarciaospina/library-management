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
            CreateMap<Book, BookUpdateDto>().ReverseMap();
            CreateMap<BookDto, BookUpdateDto>().ReverseMap(); // <-- Map explicitly BookDto -> BookUpdateDto
            CreateMap<BookUpdateDto, Book>().ReverseMap();
            CreateMap<BookCreateDto, Book>();
            CreateMap<BookDto, BookCreateDto>(); // for create, keep as-is

            // Members
            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<Member, MemberUpdateDto>().ReverseMap();
            CreateMap<MemberCreateDto, Member>();

            // Loans
            CreateMap<Loan, LoanDto>().ReverseMap();
            CreateMap<Loan, LoanUpdateDto>().ReverseMap();
            CreateMap<LoanCreateDto, Loan>();

            // Libraries
            CreateMap<Library, LibraryDto>().ReverseMap();
            CreateMap<Library, LibraryUpdateDto>().ReverseMap();
            CreateMap<LibraryCreateDto, Library>();
        }
    }
}
