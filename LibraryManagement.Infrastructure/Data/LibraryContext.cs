using LibraryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Library> Libraries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Loans)
                .WithOne(l => l.Book)
                .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.Loans)
                .WithOne(l => l.Member)
                .HasForeignKey(l => l.MemberId);

            modelBuilder.Entity<Library>()
                .HasMany(l => l.Books)
                .WithOne(b => b.Library)
                .HasForeignKey(b => b.LibraryId);

            modelBuilder.Entity<Library>()
                .HasMany(l => l.Members)
                .WithOne(m => m.Library)
                .HasForeignKey(m => m.LibraryId);
        }
    }
}
