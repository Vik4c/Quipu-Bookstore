using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Entities;
using Quipu.Bookstore.Domain.Interfaces.Books;

namespace Quipu.Bookstore.Application.Books.SearchBooks
{
    public class SearchBooksHandler(IBookRepository _bookRepository)
        : IRequestWithResultHandler<SearchBooksQuery, Result<PagedResult<BookDto>>>
    {
        public async Task<Result<PagedResult<BookDto>>> Handle(
            SearchBooksQuery request,
            CancellationToken cancellationToken)
        {
            PagedResult<Book> books =
                await _bookRepository.SearchBooksAsync(
                    title: request.Title,
                    author: request.Author,
                    page: request.Page,
                    pageSize: request.PageSize,
                    cancellationToken: cancellationToken
                );

            PagedResult<BookDto> page = new(
                items: [.. books.Items.Select(book => book.ToBookDto())],
                page: books.Page,
                pageSize: books.PageSize,
                totalCount: books.TotalCount
            );

            return Result<PagedResult<BookDto>>.Ok(page);
        }
    }
}
