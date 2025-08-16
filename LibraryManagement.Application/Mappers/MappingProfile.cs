using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Mappers
{
    /// <summary>
    /// AutoMapper profile configuration for mapping between Entities and DTOs.
    /// Includes bidirectional mappings and creation-specific mappings.
    /// Now supports extended mapping for filtering books, members, and including LibraryName in LoanDto.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // -------------------------------------------------
            // Books
            // -------------------------------------------------
            // Map Book entity to BookDto and vice versa
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId)) // Include LibraryId for filtering
                .ReverseMap();

            // Map Book entity to BookUpdateDto and vice versa
            CreateMap<Book, BookUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId)) // Preserve LibraryId for updates
                .ReverseMap();

            // NEW: Map BookDto to BookUpdateDto and vice versa
            // Useful for mapping between DTOs without affecting CRUD operations
            CreateMap<BookDto, BookUpdateDto>().ReverseMap();

            // Map BookCreateDto to Book (creation only)
            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId)); // Ensure new book has correct LibraryId

            // -------------------------------------------------
            // Members
            // -------------------------------------------------
            // Map Member entity to MemberDto and vice versa
            CreateMap<Member, MemberDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId)) // Include LibraryId for filtering members
                .ReverseMap();

            // Map Member entity to MemberUpdateDto and vice versa
            CreateMap<Member, MemberUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId)) // Preserve LibraryId for updates
                .ReverseMap();

            // Map MemberCreateDto to Member (creation only)
            CreateMap<MemberCreateDto, Member>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId)); // Assign correct LibraryId on creation

            // -------------------------------------------------
            // Loans
            // -------------------------------------------------
            // Map Loan entity to LoanDto and vice versa
            CreateMap<Loan, LoanDto>()
                // Map Book title for display
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                // Combine Member first and last names
                .ForMember(dest => dest.MemberFullName, opt => opt.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                // Map related Library name for display in Index
                .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Book.Library.Name))
                // Map related LibraryId for internal filtering or edits
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.Book.LibraryId))
                .ReverseMap();

            // Map Loan entity to LoanUpdateDto and vice versa
            // Ensures LibraryId is populated correctly from the related Book
            CreateMap<Loan, LoanUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.Book.LibraryId))
                .ReverseMap();

            // Map LoanCreateDto to Loan (creation only)
            CreateMap<LoanCreateDto, Loan>();

            // -------------------------------------------------
            // Libraries
            // -------------------------------------------------
            // Map Library entity to LibraryDto and vice versa
            CreateMap<Library, LibraryDto>().ReverseMap();

            // Map Library entity to LibraryUpdateDto and vice versa
            CreateMap<Library, LibraryUpdateDto>().ReverseMap();

            // Map LibraryCreateDto to Library (creation only)
            CreateMap<LibraryCreateDto, Library>();
        }
    }
}
