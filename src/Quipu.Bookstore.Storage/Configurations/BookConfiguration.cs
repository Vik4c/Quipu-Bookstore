using Quipu.Bookstore.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quipu.Bookstore.Storage.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books")
                .HasKey(book => book.BookId);

            builder.Property(book => book.BookId)
                .ValueGeneratedOnAdd();

            builder.Property(book => book.Title)
                .IsRequired()
                .HasMaxLength(Book.TitleMaxLength);

            builder.HasOne(book => book.Author)
                .WithMany()
                .HasForeignKey(book => book.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(book => book.Title);
            builder.HasIndex(book => book.AuthorId);

            builder.HasData(
                new { BookId = 1, Title = "Nineteen Eighty-Four", SubTitle = (string?)null, AuthorId = 1 },
                new { BookId = 2, Title = "Animal Farm", SubTitle = "A Fairy Story", AuthorId = 1 },
                new { BookId = 3, Title = "Pride and Prejudice", SubTitle = (string?)null, AuthorId = 2 },
                new { BookId = 4, Title = "Sense and Sensibility", SubTitle = (string?)null, AuthorId = 2 },
                new { BookId = 5, Title = "The Hobbit", SubTitle = "There and Back Again", AuthorId = 3 },
                new { BookId = 6, Title = "The Fellowship of the Ring", SubTitle = "Being the First Part of The Lord of the Rings", AuthorId = 3 }
            );
        }
    }
}
