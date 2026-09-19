using Quipu.Bookstore.Api.Authorization;

using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Quipu.Bookstore.Api.Authentication
{
    public static class AccessTokenPrincipalFactory
    {
        public static ClaimsPrincipal Create(string subject, ImmutableArray<string> scopes, string? name = null)
        {
            ClaimsIdentity identity = new(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role
            );

            identity.SetClaim(Claims.Subject, subject);

            if (name is not null)
                identity.SetClaim(Claims.Name, name);

            identity.SetScopes(scopes);
            identity.SetAudiences(BookstoreAudience.Api);

            foreach (Claim claim in identity.Claims)
                claim.SetDestinations(Destinations.AccessToken);

            return new ClaimsPrincipal(identity);
        }
    }
}
