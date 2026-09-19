using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;

namespace Quipu.Bookstore.Application.Books.UpdateBook
{
    public class UpdateBookCommand(int bookId, string title, string? subTitle, int authorId)
        : IRequestWithResult<Result>
    {
        public int BookId { get; } = bookId;

        public string Title { get; } = title;

        public string? SubTitle { get; } = subTitle;

        public int AuthorId { get; } = authorId;
    }
}
