namespace Quipu.Bookstore.Domain.Interfaces.Authors
{
    public interface IAuthorRepository
    {
        Task<bool> AuthorExistsAsync(int authorId, CancellationToken cancellationToken);
    }
}
