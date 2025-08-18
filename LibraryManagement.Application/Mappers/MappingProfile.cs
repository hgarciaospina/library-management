using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Mappers
{
    /// <summary>
    /// AutoMapper profile configuration for mapping between Entities and DTOs.
    /// Includes bidirectional mappings and creation-specific mappings.
    /// Now supports extended mapping for LoanDetailsDto to properly display Loan with related Member, Book, and Library info.
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
            // Map Loan entity to LoanDto
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

            // -------------------------------------------------
            // NEW: LoanDetailsDto Mapping
            // -------------------------------------------------
            // Enables mapping for detailed view of a Loan including full Member, Book, and Library information
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
