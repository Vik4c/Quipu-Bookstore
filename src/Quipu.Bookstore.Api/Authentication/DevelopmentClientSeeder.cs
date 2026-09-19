using Quipu.Bookstore.Api.Authorization;

using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Quipu.Bookstore.Api.Authentication
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

            await EnsureScopeAsync(
                scopeManager: scopeManager,
                scopeName: BookstoreScopes.BooksManage,
                cancellationToken: cancellationToken
            );

            await EnsureScopeAsync(
                scopeManager: scopeManager,
                scopeName: BookstoreScopes.BooksRead,
                cancellationToken: cancellationToken
            );

            if (string.IsNullOrWhiteSpace(options.CrudClientSecret))
                logger.LogWarning(
                    "DevelopmentAuth:CrudClientSecret is not configured. The client_credentials client was not seeded; " +
                    "set it in appsettings.Development.json, user-secrets or the DevelopmentAuth__CrudClientSecret environment variable."
                );
            else
                await UpsertApplicationAsync(
                    applicationManager: applicationManager,
                    descriptor: CrudClient(options.CrudClientSecret),
                    cancellationToken: cancellationToken
                );

            string[] redirectUris = configuration.GetSection("Oidc:SwaggerRedirectUris").Get<string[]>() ?? [];

            if (redirectUris.Length == 0)
                logger.LogWarning("Oidc:SwaggerRedirectUris is empty. The implicit-flow Swagger client cannot redirect back after sign-in.");

            await UpsertApplicationAsync(
                applicationManager: applicationManager,
                descriptor: SwaggerClient(redirectUris),
                cancellationToken: cancellationToken
            );
        }

        private static async Task EnsureScopeAsync(
            IOpenIddictScopeManager scopeManager,
            string scopeName,
            CancellationToken cancellationToken)
        {
            if (await scopeManager.FindByNameAsync(scopeName, cancellationToken) is not null)
                return;

            await scopeManager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    Name = scopeName,
                    Resources = { BookstoreAudience.Api }
                },
                cancellationToken
            );
        }

        private static async Task UpsertApplicationAsync(
            IOpenIddictApplicationManager applicationManager,
            OpenIddictApplicationDescriptor descriptor,
            CancellationToken cancellationToken)
        {
            object? application = await applicationManager.FindByClientIdAsync(descriptor.ClientId!, cancellationToken);

            if (application is null)
            {
                await applicationManager.CreateAsync(descriptor, cancellationToken);
                return;
            }

            await applicationManager.UpdateAsync(application, descriptor, cancellationToken);
        }

        private static OpenIddictApplicationDescriptor CrudClient(string clientSecret)
            => new()
            {
                ClientId = BookstoreClients.CrudClientId,
                ClientSecret = clientSecret,
                ClientType = ClientTypes.Confidential,
                DisplayName = "Quipu Bookstore CRUD client (client_credentials)",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.Prefixes.Scope + BookstoreScopes.BooksManage
                }
            };

        private static OpenIddictApplicationDescriptor SwaggerClient(IEnumerable<string> redirectUris)
        {
            OpenIddictApplicationDescriptor descriptor = new()
            {
                ClientId = BookstoreClients.SwaggerClientId,
                ClientType = ClientTypes.Public,
                DisplayName = "Quipu Bookstore Swagger client (implicit)",
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.GrantTypes.Implicit,
                    Permissions.ResponseTypes.Token,
                    Permissions.Prefixes.Scope + BookstoreScopes.BooksRead
                }
            };

            foreach (string redirectUri in redirectUris)
                descriptor.RedirectUris.Add(new Uri(redirectUri));

            return descriptor;
        }
    }
}
