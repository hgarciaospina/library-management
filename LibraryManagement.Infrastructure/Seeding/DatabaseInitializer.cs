using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Seeding
{
    /// <summary>
    /// Initializes the database with a clean migration and seed data.
    /// - Ensures database is created according to EF migrations.
    /// - Seeds Libraries, Members, Books, and sample Loans if tables are empty.
    /// - Safe for subsequent executions (does not delete existing real data).
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(LibraryContext context)
        {
            Console.WriteLine("Starting database initialization...");

            // Apply migrations (creates DB if it does not exist)
            Console.WriteLine("Applying pending migrations...");
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");

            // ======================
            // Libraries
            // ======================
            if (!context.Libraries.Any())
            {
                var libraries = new[]
                {
                    new Library { Name = "Central Library", Address = "123 Main St" },
                    new Library { Name = "East Branch", Address = "456 East Ave" }
                };

                context.Libraries.AddRange(libraries);
                context.SaveChanges();
                Console.WriteLine($"Seeded {libraries.Length} libraries.");
            }
            else
            {
                Console.WriteLine("Libraries already exist. Skipping seeding libraries.");
            }

            // ======================
            // Members
            // ======================
            if (!context.Members.Any())
            {
                var centralLibrary = context.Libraries.First(l => l.Name == "Central Library");
                var eastBranch = context.Libraries.First(l => l.Name == "East Branch");

                var members = new[]
                {
                    new Member { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", LibraryId = centralLibrary.Id },
                    new Member { FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", LibraryId = centralLibrary.Id },
                    new Member { FirstName = "Carol", LastName = "Davis", Email = "carol@example.com", LibraryId = eastBranch.Id },
                    new Member { FirstName = "David", LastName = "Miller", Email = "david@example.com", LibraryId = eastBranch.Id }
                };

                context.Members.AddRange(members);
                context.SaveChanges();
                Console.WriteLine($"Seeded {members.Length} members.");
            }
            else
            {
                Console.WriteLine("Members already exist. Skipping seeding members.");
            }

            // ======================
            // Books
            // ======================
            if (!context.Books.Any())
            {
                var centralLibrary = context.Libraries.First(l => l.Name == "Central Library");
                var eastBranch = context.Libraries.First(l => l.Name == "East Branch");

                var books = new[]
                {
                    new Book { Title = "C# in Depth", Author = "Jon Skeet", ISBN = "1234567890", PublicationYear = 2020, LibraryId = centralLibrary.Id, IsAvailable = true },
                    new Book { Title = "Clean Code", Author = "Robert C. Martin", ISBN = "0987654321", PublicationYear = 2019, LibraryId = centralLibrary.Id, IsAvailable = true },
                    new Book { Title = "Effective .NET", Author = "Bill Wagner", ISBN = "1122334455", PublicationYear = 2018, LibraryId = centralLibrary.Id, IsAvailable = true },
                    new Book { Title = "Pro ASP.NET Core", Author = "Adam Freeman", ISBN = "6677889900", PublicationYear = 2021, LibraryId = centralLibrary.Id, IsAvailable = true },

                    new Book { Title = "Java in Action", Author = "John Doe", ISBN = "2233445566", PublicationYear = 2017, LibraryId = eastBranch.Id, IsAvailable = true },
                    new Book { Title = "Spring Boot Guide", Author = "Jane Smith", ISBN = "3344556677", PublicationYear = 2019, LibraryId = eastBranch.Id, IsAvailable = true },
                    new Book { Title = "Hibernate Basics", Author = "Mary Johnson", ISBN = "4455667788", PublicationYear = 2016, LibraryId = eastBranch.Id, IsAvailable = true },
                    new Book { Title = "Clean Architecture", Author = "Robert C. Martin", ISBN = "5566778899", PublicationYear = 2018, LibraryId = eastBranch.Id, IsAvailable = true }
                };

                context.Books.AddRange(books);
                context.SaveChanges();
                Console.WriteLine($"Seeded {books.Length} books.");
            }
            else
            {
                Console.WriteLine("Books already exist. Skipping seeding books.");
            }

            // ======================
            // Sample Loans
            // ======================
            if (!context.Loans.Any())
            {
                var members = context.Members.ToList();
                var books = context.Books.ToList();

                var loans = new[]
                {
                    new Loan { MemberId = members[0].Id, BookId = books[0].Id, LoanDate = DateTime.Now.AddDays(-10), DueDate = DateTime.Now.AddDays(10), ReturnDate = null },
                    new Loan { MemberId = members[1].Id, BookId = books[1].Id, LoanDate = DateTime.Now.AddDays(-15), DueDate = DateTime.Now.AddDays(-1), ReturnDate = DateTime.Now.AddDays(-2) },
                    new Loan { MemberId = members[2].Id, BookId = books[4].Id, LoanDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(5), ReturnDate = null },
                    new Loan { MemberId = members[3].Id, BookId = books[5].Id, LoanDate = DateTime.Now.AddDays(-7), DueDate = DateTime.Now.AddDays(3), ReturnDate = null }
                };

                // Mark loaned books as unavailable
                foreach (var loan in loans.Where(l => !l.ReturnDate.HasValue))
                {
                    var book = books.First(b => b.Id == loan.BookId);
                    book.IsAvailable = false;
                }

                context.Loans.AddRange(loans);
                context.SaveChanges();
                Console.WriteLine($"Seeded {loans.Length} loans.");
            }
            else
            {
                Console.WriteLine("Loans already exist. Skipping seeding loans.");
            }

            Console.WriteLine("Database initialization completed successfully.");
        }
    }
}
