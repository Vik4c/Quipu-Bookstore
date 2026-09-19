using Quipu.Bookstore.Domain.Entities;

using System.ComponentModel.DataAnnotations;

namespace Quipu.Bookstore.Contracts.Books
{
    public class UpdateBookRequest
    {
        [Required]
        [StringLength(Book.TitleMaxLength, MinimumLength = Book.TitleMinLength)]
        public required string Title { get; init; }

        public string? SubTitle { get; init; }

        [Range(1, int.MaxValue)]
        public required int AuthorId { get; init; }
    }
}
