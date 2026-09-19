using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace Quipu.Bookstore.Api.Authentication.Tokens
{
    public class ClientCredentialsTokenHandler : IOpenIddictServerHandler<HandleTokenRequestContext>
    {
        public ValueTask HandleAsync(HandleTokenRequestContext context)
        {
            if (!context.Request.IsClientCredentialsGrantType())
                return default;

            context.SignIn(AccessTokenPrincipalFactory.Create(
                subject: context.Request.ClientId!,
                scopes: context.Request.GetScopes()
            ));

            return default;
        }
    }
}
