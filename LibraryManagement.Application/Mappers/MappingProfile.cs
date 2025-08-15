using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Mappers
{
    /// <summary>
    /// AutoMapper profile configuration for mapping between Entities and DTOs.
    /// Includes bidirectional mappings and creation-specific mappings.
    /// Now supports extended mapping for filtering books and members by LibraryId.
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
                // Ensure LibraryId is included for filtering
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            // Map Book entity to BookUpdateDto and vice versa
            CreateMap<Book, BookUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            // ✅ NEW: Map BookDto to BookUpdateDto and vice versa
            // This mapping is required to allow _mapper.Map<BookUpdateDto>(bookDto)
            // without causing "Missing type map configuration" errors.
            // This does NOT affect the existing Books CRUD.
            CreateMap<BookDto, BookUpdateDto>().ReverseMap();

            // Map BookCreateDto to Book (creation only)
            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId));

            // -------------------------------------------------
            // Members
            // -------------------------------------------------
            // Map Member entity to MemberDto and vice versa
            CreateMap<Member, MemberDto>()
                // Include LibraryId to allow filtering members by library
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            // Map Member entity to MemberUpdateDto and vice versa
            CreateMap<Member, MemberUpdateDto>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId))
                .ReverseMap();

            // Map MemberCreateDto to Member (creation only)
            CreateMap<MemberCreateDto, Member>()
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.LibraryId));

            // -------------------------------------------------
            // Loans
            // -------------------------------------------------
            // Map Loan entity to LoanDto and vice versa
            CreateMap<Loan, LoanDto>()
                // Ensure we map related LibraryId for filtering in UI if needed
                .ForMember(dest => dest.LibraryId, opt => opt.MapFrom(src => src.Book.LibraryId))
                .ReverseMap();

            // Map Loan entity to LoanUpdateDto and vice versa
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
