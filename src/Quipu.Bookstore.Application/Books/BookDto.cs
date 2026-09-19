using Quipu.Bookstore.Application.Authors;

namespace Quipu.Bookstore.Application.Books
{
    public class BookDto
    {
        public int BookId { get; init; }

        public required string Title { get; init; }

        public string? SubTitle { get; init; }

        public required AuthorDto Author { get; init; }
    }
}
