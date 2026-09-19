using Quipu.Bookstore.Api.Authentication.Login;
using Quipu.Bookstore.Api.Authentication.Tokens;

namespace Quipu.Bookstore.Api.Authentication
{
    public static class AuthenticationEndpoints
    {
        public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("connect/login", LoginPage.Render);
            endpoints.MapPost("connect/login", LoginSubmitHandler.HandleAsync);

            // A method group here would bind to the RequestDelegate overload and discard the IResult (ASP0016).
            endpoints.MapGet("connect/authorize", async (HttpContext httpContext) => await AuthorizeRequestHandler.HandleAsync(httpContext));

            return endpoints;
        }
    }
}
