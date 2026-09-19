using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace Quipu.Bookstore.Api.Authentication.Tokens
{
    public static class AuthorizeRequestHandler
    {
        public static async Task<IResult> HandleAsync(HttpContext httpContext)
        {
            OpenIddictRequest request = httpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            AuthenticateResult cookieResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!cookieResult.Succeeded || cookieResult.Principal?.Identity?.Name is null)
            {
                await httpContext.ChallengeAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new AuthenticationProperties
                    {
                        RedirectUri = httpContext.Request.GetEncodedPathAndQuery()
                    }
                );

                return Results.Empty;
            }

            string username = cookieResult.Principal.Identity.Name;

            ClaimsPrincipal principal = AccessTokenPrincipalFactory.Create(
                subject: username,
                scopes: request.GetScopes(),
                name: username
            );

            await httpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);

            return Results.Empty;
        }
    }
}
