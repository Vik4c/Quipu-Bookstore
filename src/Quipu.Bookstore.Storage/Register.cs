using Quipu.Bookstore.Domain.Interfaces.Authors;
using Quipu.Bookstore.Domain.Interfaces.Books;
using Quipu.Bookstore.Storage.Context;
using Quipu.Bookstore.Storage.Repository.Authors;
using Quipu.Bookstore.Storage.Repository.Books;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Quipu.Bookstore.Storage
{
    public static partial class Register
    {
        public static IServiceCollection RegisterStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("BookstoreDatabase");

            services.AddDbContext<AppDbContext>(options =>
            {
                if (!string.IsNullOrWhiteSpace(connectionString))
                    options.UseSqlServer(connectionString);
            });

            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();

            services
                .AddOpenIddict()
                .AddCore(options => options
                    .UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>());

            return services;
        }
    }
}
