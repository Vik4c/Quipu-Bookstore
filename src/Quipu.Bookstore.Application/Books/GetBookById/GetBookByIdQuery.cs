using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;

namespace Quipu.Bookstore.Application.Books.GetBookById
{
    public class GetBookByIdQuery(int bookId) : IRequestWithResult<Result<BookDto>>
    {
        public int BookId { get; } = bookId;
    }
}
