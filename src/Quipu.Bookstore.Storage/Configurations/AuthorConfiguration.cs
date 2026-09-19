using Quipu.Bookstore.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quipu.Bookstore.Storage.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors")
                .HasKey(author => author.AuthorId);

            builder.Property(author => author.AuthorId)
                .ValueGeneratedOnAdd();

            builder.Property(author => author.Name)
                .IsRequired()
                .HasMaxLength(Author.NameMaxLength);

            builder.HasIndex(author => author.Name);

            builder.HasData(
                new { AuthorId = 1, Name = "George Orwell" },
                new { AuthorId = 2, Name = "Jane Austen" },
                new { AuthorId = 3, Name = "J.R.R. Tolkien" }
            );
        }
    }
}
