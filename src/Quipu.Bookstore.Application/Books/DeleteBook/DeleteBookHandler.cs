using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Interfaces.Books;

namespace Quipu.Bookstore.Application.Books.DeleteBook
{
    public class DeleteBookHandler(IBookRepository _bookRepository)
        : IRequestWithResultHandler<DeleteBookCommand, Result>
    {
        public async Task<Result> Handle(
            DeleteBookCommand request,
            CancellationToken cancellationToken)
        {
            return await _bookRepository.DeleteBookAsync(
                bookId: request.BookId,
                cancellationToken: cancellationToken
            );
        }
    }
}
