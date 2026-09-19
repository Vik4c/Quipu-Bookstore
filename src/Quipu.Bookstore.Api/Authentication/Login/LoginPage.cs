namespace Quipu.Bookstore.Api.Authentication.Login
{
    public static class LoginPage
    {
        public static IResult Render(HttpContext httpContext)
        {
            string returnUrl = httpContext.Request.Query["ReturnUrl"].ToString();
            string error = httpContext.Request.Query["error"].ToString();
            string errorMessage = error == "invalid_credentials"
                ? "Invalid username or password."
                : string.Empty;
            string errorMarkup = string.IsNullOrEmpty(errorMessage)
                ? string.Empty
                : $"""<div class="error" role="alert">{errorMessage}</div>""";

            string html = $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <title>Quipu Bookstore — Development Sign-in</title>
                    <style>
                        body { font-family: system-ui, sans-serif; background: #f5f5f5; display: flex; justify-content: center; padding: 3rem 1rem; margin: 0; }
                        .card { background: #fff; border-radius: 8px; padding: 2rem; max-width: 360px; width: 100%; box-shadow: 0 1px 3px rgba(0,0,0,.12); box-sizing: border-box; }
                        h1 { font-size: 1.25rem; margin: 0 0 .25rem; }
                        .badge { display: inline-block; font-size: .75rem; color: #92400e; background: #fef3c7; padding: .15rem .5rem; border-radius: 4px; margin-bottom: 1rem; }
                        .error { background: #fee2e2; color: #991b1b; padding: .5rem .75rem; border-radius: 4px; font-size: .9rem; margin-bottom: 1rem; }
                        label { display: block; font-size: .85rem; margin-bottom: .25rem; }
                        input { width: 100%; padding: .5rem; margin-bottom: .75rem; box-sizing: border-box; border: 1px solid #d1d5db; border-radius: 4px; }
                        button { width: 100%; padding: .6rem; background: #1f2937; color: #fff; border: none; border-radius: 4px; cursor: pointer; }
                    </style>
                </head>
                <body>
                    <div class="card">
                        <span class="badge">Local development only</span>
                        <h1>Quipu Bookstore Sign-in</h1>
                        <p>This page exists only to demonstrate the OAuth2 implicit flow in local development.</p>
                        {{errorMarkup}}
                        <form method="post" action="/connect/login?ReturnUrl={{Uri.EscapeDataString(returnUrl)}}">
                            <label for="username">Username</label>
                            <input id="username" type="text" name="username" autocomplete="username" />
                            <label for="password">Password</label>
                            <input id="password" type="password" name="password" autocomplete="current-password" />
                            <button type="submit">Sign in</button>
                        </form>
                    </div>
                </body>
                </html>
                """;

            return Results.Content(html, "text/html");
        }
    }
}
