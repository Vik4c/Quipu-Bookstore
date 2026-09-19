using Quipu.Bookstore.Api.Authorization;

using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Quipu.Bookstore.Api.Authentication.Seeding
{
    public static class DevelopmentClientSeeder
    {
        public static async Task SeedDevelopmentClientsAsync(
            this IServiceProvider services,
            CancellationToken cancellationToken = default)
        {
            using IServiceScope scope = services.CreateScope();

            DevelopmentAuthOptions options = scope.ServiceProvider.GetRequiredService<IOptions<DevelopmentAuthOptions>>().Value;
            IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            IOpenIddictScopeManager scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            IOpenIddictApplicationManager applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DevelopmentClientSeeder));

            await scopeManager.EnsureScopeAsync(
                scopeName: BookstoreScopes.BooksManage,
                cancellationToken: cancellationToken
            );

            await scopeManager.EnsureScopeAsync(
                scopeName: BookstoreScopes.BooksRead,
                cancellationToken: cancellationToken
            );

            if (string.IsNullOrWhiteSpace(options.CrudClientSecret))
                logger.LogWarning(
                    "DevelopmentAuth:CrudClientSecret is not configured. The client_credentials client was not seeded; " +
                    "set it in appsettings.Development.json, user-secrets or the DevelopmentAuth__CrudClientSecret environment variable."
                );
            else
                await applicationManager.UpsertApplicationAsync(
                    descriptor: DevelopmentClientDescriptors.CrudClient(options.CrudClientSecret),
                    cancellationToken: cancellationToken
                );

            string[] redirectUris = configuration.GetSection("Oidc:SwaggerRedirectUris").Get<string[]>() ?? [];

            if (redirectUris.Length == 0)
                logger.LogWarning("Oidc:SwaggerRedirectUris is empty. The implicit-flow Swagger client cannot redirect back after sign-in.");

            await applicationManager.UpsertApplicationAsync(
                descriptor: DevelopmentClientDescriptors.SwaggerClient(redirectUris),
                cancellationToken: cancellationToken
            );
        }
    }
}
