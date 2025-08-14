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
            CreateMap<Book, BookUpdateDto>().ReverseMap(); // <--- Añadido para mapear Book <-> BookUpdateDto
            CreateMap<BookDto, BookUpdateDto>().ReverseMap(); // Mapeo bidireccional
            CreateMap<BookUpdateDto, Book>().ReverseMap();
            CreateMap<BookUpdateDto, BookDto>(); // Opcional: si solo necesitas una dirección
            CreateMap<BookCreateDto, Book>();

            // Members
            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<Member, MemberUpdateDto>().ReverseMap(); // Map para edición
            CreateMap<MemberCreateDto, Member>();

            // Loans
            CreateMap<Loan, LoanDto>().ReverseMap();
            CreateMap<Loan, LoanUpdateDto>().ReverseMap(); // Map para edición
            CreateMap<LoanCreateDto, Loan>();

            // Libraries
            CreateMap<Library, LibraryDto>().ReverseMap();
            CreateMap<Library, LibraryUpdateDto>().ReverseMap(); // Map para edición
            CreateMap<LibraryCreateDto, Library>();
        }
    }
}
