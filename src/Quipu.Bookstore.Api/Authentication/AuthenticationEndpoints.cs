using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace Quipu.Bookstore.Api.Authentication
{
    public static class AuthenticationEndpoints
    {
        public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("connect/login", HandleLoginPage);
            endpoints.MapPost("connect/login", HandleLoginSubmitAsync);

            // A method group here would bind to the RequestDelegate overload and discard the IResult (ASP0016).
            endpoints.MapGet("connect/authorize", async (HttpContext httpContext) => await HandleAuthorizeAsync(httpContext));

            return endpoints;
        }

        private static IResult HandleLoginPage(HttpContext httpContext)
        {
            string returnUrl = httpContext.Request.Query["ReturnUrl"].ToString();
            string error = httpContext.Request.Query["error"].ToString();
            string errorMessage = error == "invalid_credentials"
                ? "Invalid username or password."
                : string.Empty;
            string errorMarkup = string.IsNullOrEmpty(errorMessage)
                ? string.Empty
                : $"<p style=\"color: red;\">{errorMessage}</p>";

            string html = $"""
                <!DOCTYPE html>
                <html>
                <head><title>Quipu Bookstore — Development Sign-in</title></head>
                <body>
                    <h3>Quipu Bookstore — Development Sign-in</h3>
                    {errorMarkup}
                    <p>This login page exists only to demonstrate the OAuth2 implicit flow in local development.</p>
                    <form method="post" action="/connect/login?ReturnUrl={Uri.EscapeDataString(returnUrl)}">
                        <label>Username <input type="text" name="username" autocomplete="username" /></label><br/>
                        <label>Password <input type="password" name="password" autocomplete="current-password" /></label><br/>
                        <button type="submit">Sign in</button>
                    </form>
                </body>
                </html>
                """;

            return Results.Content(html, "text/html");
        }

        private static async Task<IResult> HandleLoginSubmitAsync(
            HttpContext httpContext,
            IOptions<DevelopmentAuthOptions> developmentAuthOptions)
        {
            IFormCollection form = await httpContext.Request.ReadFormAsync();
            string username = form["username"].ToString();
            string password = form["password"].ToString();
            string returnUrl = httpContext.Request.Query["ReturnUrl"].ToString();

            DevelopmentAuthOptions options = developmentAuthOptions.Value;

            bool credentialsConfigured = !string.IsNullOrEmpty(options.Username) && !string.IsNullOrEmpty(options.Password);
            bool credentialsMatch = credentialsConfigured
                && string.Equals(username, options.Username, StringComparison.Ordinal)
                && string.Equals(password, options.Password, StringComparison.Ordinal);

            if (!credentialsMatch)
                return Results.Redirect($"/connect/login?ReturnUrl={Uri.EscapeDataString(returnUrl)}&error=invalid_credentials");

            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, username));

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return Results.LocalRedirect(IsLocalUrl(returnUrl) ? returnUrl : "/");
        }

        private static async Task<IResult> HandleAuthorizeAsync(HttpContext httpContext)
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

        private static bool IsLocalUrl(string url)
            => url.StartsWith('/')
                && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))
                && !url.Any(char.IsControl);
    }
}