using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Quipu.Bookstore.Storage.Context
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync(
            this IServiceProvider services,
            CancellationToken cancellationToken = default)
        {
            using IServiceScope scope = services.CreateScope();

            AppDbContext database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await database.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
