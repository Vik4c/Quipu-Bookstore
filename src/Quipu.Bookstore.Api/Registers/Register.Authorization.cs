using Quipu.Bookstore.Api.Authorization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Quipu.Bookstore.Api.Registers
{
    public static partial class Register
    {
        public static IServiceCollection RegisterAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy(BookstorePolicies.BooksManage, policy => policy.RequireClaim(Claims.Scope, BookstoreScopes.BooksManage))
                .AddPolicy(BookstorePolicies.BooksRead, policy => policy.RequireClaim(Claims.Scope, BookstoreScopes.BooksRead));

            return services;
        }
    }
}
