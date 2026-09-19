using Quipu.Bookstore.Application.Books;
using Quipu.Bookstore.Contracts.Authors;
using Quipu.Bookstore.Contracts.Books;
using Quipu.Bookstore.Contracts.Common;
using Quipu.Bookstore.Domain.Common;

namespace Quipu.Bookstore.Api.Mappings
{
    public static class BookResponseMappings
    {
        public static BookResponse ToBookResponse(this BookDto book)
            => new()
            {
                BookId = book.BookId,
                Title = book.Title,
                SubTitle = book.SubTitle,
                Author = new AuthorResponse
                {
                    AuthorId = book.Author.AuthorId,
                    Name = book.Author.Name
                }
            };

        public static PagedResponse<BookResponse> ToPagedBookResponse(this PagedResult<BookDto> page)
            => new()
            {
                Items = [.. page.Items.Select(book => book.ToBookResponse())],
                Page = page.Page,
                PageSize = page.PageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages
            };
    }
}
