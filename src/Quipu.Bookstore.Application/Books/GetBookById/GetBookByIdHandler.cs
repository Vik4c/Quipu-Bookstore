using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Entities;
using Quipu.Bookstore.Domain.Interfaces.Books;

namespace Quipu.Bookstore.Application.Books.GetBookById
{
    public class GetBookByIdHandler(IBookRepository _bookRepository)
        : IRequestWithResultHandler<GetBookByIdQuery, Result<BookDto>>
    {
        public async Task<Result<BookDto>> Handle(
            GetBookByIdQuery request,
            CancellationToken cancellationToken)
        {
            Result<Book> book =
                await _bookRepository.GetBookByIdAsync(
                    bookId: request.BookId,
                    cancellationToken: cancellationToken
                );

            if (!book.IsSuccess)
                return Result<BookDto>.NotFound(
                    error: book.Error,
                    errorCode: book.ErrorCode
                );

            return Result<BookDto>.Ok(book.Value.ToBookDto());
        }
    }
}
