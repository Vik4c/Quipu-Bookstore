using Quipu.Bookstore.Domain.Common;
using Quipu.Bookstore.Domain.Entities;
using Quipu.Bookstore.Domain.Interfaces.Books;
using Quipu.Bookstore.Storage.Context;

using Microsoft.EntityFrameworkCore;

namespace Quipu.Bookstore.Storage.Repository.Books
{
    public class BookRepository(AppDbContext _dbContext) : IBookRepository
    {
        public async Task<Result<Book>> GetBookByIdAsync(int bookId, CancellationToken cancellationToken)
        {
            Book? book = await _dbContext.Books
                .AsNoTracking()
                .Include(entity => entity.Author)
                .FirstOrDefaultAsync(entity => entity.BookId == bookId, cancellationToken);

            if (book is null)
                return BookNotFound(bookId);

            return Result<Book>.Ok(book);
        }

        public async Task<Result<Book>> GetBookForUpdateAsync(int bookId, CancellationToken cancellationToken)
        {
            Book? book = await _dbContext.Books
                .FirstOrDefaultAsync(entity => entity.BookId == bookId, cancellationToken);

            if (book is null)
                return BookNotFound(bookId);

            return Result<Book>.Ok(book);
        }

        public async Task<PagedResult<Book>> SearchBooksAsync(
            string? title,
            string? author,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            IQueryable<Book> query = _dbContext.Books
                .AsNoTracking()
                .Include(entity => entity.Author);

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(entity => EF.Functions.Like(entity.Title, $"%{title}%"));

            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(entity => EF.Functions.Like(entity.Author.Name, $"%{author}%"));

            int totalCount = await query.CountAsync(cancellationToken);

            List<Book> books = await query
                .OrderBy(entity => entity.Title)
                .ThenBy(entity => entity.BookId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Book>(
                items: books,
                page: page,
                pageSize: pageSize,
                totalCount: totalCount
            );
        }

        public async Task<Book> AddBookAsync(Book book, CancellationToken cancellationToken)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Entry(book)
                .Reference(entity => entity.Author)
                .LoadAsync(cancellationToken);

            return book;
        }

        public async Task UpdateBookAsync(Book book, CancellationToken cancellationToken)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Result> DeleteBookAsync(int bookId, CancellationToken cancellationToken)
        {
            int deleted = await _dbContext.Books
                .Where(entity => entity.BookId == bookId)
                .ExecuteDeleteAsync(cancellationToken);

            if (deleted == 0)
                return BookNotFound(bookId);

            return Result.Ok();
        }

        private static Result<Book> BookNotFound(int bookId)
            => Result<Book>.NotFound(
                error: $"Book {bookId} was not found.",
                errorCode: ResultCodes.BOOK_NOT_FOUND
            );
    }
}
