using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Entities;

namespace Quipu.Bookstore.Domain.Interfaces.Books
{
    public interface IBookRepository
    {
        Task<Result<Book>> GetBookByIdAsync(int bookId, CancellationToken cancellationToken);

        Task<Result<Book>> GetBookForUpdateAsync(int bookId, CancellationToken cancellationToken);

        Task<PagedResult<Book>> SearchBooksAsync(string? title, string? author, int page, int pageSize, CancellationToken cancellationToken);

        Task<Book> AddBookAsync(Book book, CancellationToken cancellationToken);

        Task UpdateBookAsync(Book book, CancellationToken cancellationToken);

        Task<Result> DeleteBookAsync(int bookId, CancellationToken cancellationToken);
    }
}
