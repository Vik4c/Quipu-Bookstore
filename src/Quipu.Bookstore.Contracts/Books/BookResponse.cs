using Quipu.Bookstore.Contracts.Authors;

namespace Quipu.Bookstore.Contracts.Books
{
    public class BookResponse
    {
        public int BookId { get; init; }

        public required string Title { get; init; }

        public string? SubTitle { get; init; }

        public required AuthorResponse Author { get; init; }
    }
}
