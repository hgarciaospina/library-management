using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Application.Mappers
{
    /// <summary>
    /// Configuración de AutoMapper para mapeo entre entidades y DTOs.
    /// Incluye Books, Members, Loans y Libraries.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // -------------------------------------------------
            // Books
            // -------------------------------------------------
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library.Name))
                .ReverseMap();

            CreateMap<Book, BookUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            CreateMap<BookDto, BookUpdateDto>().ReverseMap();

            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId));

            // -------------------------------------------------
            // Members
            // -------------------------------------------------
            CreateMap<Member, MemberDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            CreateMap<Member, MemberUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            CreateMap<MemberCreateDto, Member>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId));

            // -------------------------------------------------
            // Loans
            // -------------------------------------------------
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.MemberFullName, opt => opt.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Book.Library.Name))
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.Book.LibraryId))
                .ReverseMap();

            CreateMap<Loan, LoanUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.Book.LibraryId))
                .ReverseMap();

            CreateMap<LoanCreateDto, Loan>();

            // LoanDetailsDto para vistas detalladas
            CreateMap<Loan, LoanDetailsDto>()
                .ForMember(dest => dest.MemberFullName, opt => opt.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN))
                .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Book.Library.Name))
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.Book.LibraryId));

            // -------------------------------------------------
            // Libraries
            // -------------------------------------------------
            CreateMap<Library, LibraryDto>().ReverseMap();
            CreateMap<Library, LibraryUpdateDto>().ReverseMap();
            CreateMap<LibraryCreateDto, Library>();
        }
    }
}
