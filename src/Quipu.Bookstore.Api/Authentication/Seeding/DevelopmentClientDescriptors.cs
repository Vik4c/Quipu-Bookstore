using Quipu.Bookstore.Api.Authorization;

using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Quipu.Bookstore.Api.Authentication.Seeding
{
    public static class DevelopmentClientDescriptors
    {
        public static OpenIddictApplicationDescriptor CrudClient(string clientSecret)
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

        public static OpenIddictApplicationDescriptor SwaggerClient(IEnumerable<string> redirectUris)
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
