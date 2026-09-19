using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Entities;
using Quipu.Bookstore.Domain.Interfaces.Authors;
using Quipu.Bookstore.Domain.Interfaces.Books;

namespace Quipu.Bookstore.Application.Books.UpdateBook
{
    public class UpdateBookHandler(
        IBookRepository _bookRepository,
        IAuthorRepository _authorRepository)
        : IRequestWithResultHandler<UpdateBookCommand, Result>
    {
        public async Task<Result> Handle(
            UpdateBookCommand request,
            CancellationToken cancellationToken)
        {
            Result<Book> book =
                await _bookRepository.GetBookForUpdateAsync(
                    bookId: request.BookId,
                    cancellationToken: cancellationToken
                );

            if (!book.IsSuccess)
                return book;

            bool authorExists =
                await _authorRepository.AuthorExistsAsync(
                    authorId: request.AuthorId,
                    cancellationToken: cancellationToken
                );

            if (!authorExists)
                return Result.Invalid(
                    error: $"Author {request.AuthorId} does not exist.",
                    errorCode: ResultCodes.AUTHOR_NOT_FOUND
                );

            book.Value.UpdateDetails(
                title: request.Title,
                subTitle: request.SubTitle,
                authorId: request.AuthorId
            );

            await _bookRepository.UpdateBookAsync(
                book: book.Value,
                cancellationToken: cancellationToken
            );

            return Result.Ok();
        }
    }
}
