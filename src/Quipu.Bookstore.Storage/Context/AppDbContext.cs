using Quipu.Bookstore.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Quipu.Bookstore.Storage.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.UseOpenIddict();

            base.OnModelCreating(modelBuilder);
        }
    }
}
