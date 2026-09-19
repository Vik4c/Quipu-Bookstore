using Quipu.Bookstore.Api.Authentication.Seeding;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Quipu.Bookstore.Api.Authentication.Login
{
    public static class LoginSubmitHandler
    {
        public static async Task<IResult> HandleAsync(
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

        private static bool IsLocalUrl(string url)
            => url.StartsWith('/')
                && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))
                && !url.Any(char.IsControl);
    }
}
