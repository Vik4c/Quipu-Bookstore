using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Entities;
using Quipu.Bookstore.Domain.Interfaces.Authors;
using Quipu.Bookstore.Domain.Interfaces.Books;

namespace Quipu.Bookstore.Application.Books.CreateBook
{
    public class CreateBookHandler(
        IBookRepository _bookRepository,
        IAuthorRepository _authorRepository)
        : IRequestWithResultHandler<CreateBookCommand, Result<BookDto>>
    {
        public async Task<Result<BookDto>> Handle(
            CreateBookCommand request,
            CancellationToken cancellationToken)
        {
            bool authorExists =
                await _authorRepository.AuthorExistsAsync(
                    authorId: request.AuthorId,
                    cancellationToken: cancellationToken
                );

            if (!authorExists)
                return Result<BookDto>.Invalid(
                    error: $"Author {request.AuthorId} does not exist.",
                    errorCode: ResultCodes.AUTHOR_NOT_FOUND
                );

            Book book = new(
                title: request.Title,
                subTitle: request.SubTitle,
                authorId: request.AuthorId
            );

            Book created =
                await _bookRepository.AddBookAsync(
                    book: book,
                    cancellationToken: cancellationToken
                );

            return Result<BookDto>.Ok(created.ToBookDto());
        }
    }
}
