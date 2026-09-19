using Quipu.Bookstore.Application.Abstractions;
using Quipu.Bookstore.Domain.Common;

namespace Quipu.Bookstore.Application.Books.SearchBooks
{
    public class SearchBooksQuery(string? title, string? author, int page, int pageSize)
        : IRequestWithResult<Result<PagedResult<BookDto>>>
    {
        public string? Title { get; } = title;

        public string? Author { get; } = author;

        public int Page { get; } = page;

        public int PageSize { get; } = pageSize;
    }
}
