using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;

namespace Quipu.Bookstore.Application.Books.CreateBook
{
    public class CreateBookCommand(string title, string? subTitle, int authorId)
        : IRequestWithResult<Result<BookDto>>
    {
        public string Title { get; } = title;

        public string? SubTitle { get; } = subTitle;

        public int AuthorId { get; } = authorId;
    }
}
