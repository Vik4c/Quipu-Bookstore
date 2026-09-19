using Quipu.Bookstore.Application.Authors;
using Quipu.Bookstore.Domain.Entities;

namespace Quipu.Bookstore.Application.Books
{
    public static class BookMappings
    {
        public static BookDto ToBookDto(this Book book)
            => new()
            {
                BookId = book.BookId,
                Title = book.Title,
                SubTitle = book.SubTitle,
                Author = new AuthorDto
                {
                    AuthorId = book.Author.AuthorId,
                    Name = book.Author.Name
                }
            };
    }
}
