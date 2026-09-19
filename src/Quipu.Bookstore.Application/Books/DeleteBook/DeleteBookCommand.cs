using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;

namespace Quipu.Bookstore.Application.Books.DeleteBook
{
    public class DeleteBookCommand(int bookId) : IRequestWithResult<Result>
    {
        public int BookId { get; } = bookId;
    }
}
