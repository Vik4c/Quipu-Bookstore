using Quipu.Bookstore.Domain.Interfaces.Authors;
using Quipu.Bookstore.Storage.Context;

using Microsoft.EntityFrameworkCore;

namespace Quipu.Bookstore.Storage.Repository.Authors
{
    public class AuthorRepository(AppDbContext _dbContext) : IAuthorRepository
    {
        public async Task<bool> AuthorExistsAsync(int authorId, CancellationToken cancellationToken) =>
            await _dbContext.Authors.AnyAsync(author => author.AuthorId == authorId, cancellationToken);
    }
}
