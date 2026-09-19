using Quipu.Bookstore.Api.Authorization;
using Quipu.Bookstore.Api.Swagger;

using Microsoft.OpenApi.Models;

namespace Quipu.Bookstore.Api.Registers
{
    public static partial class Register
    {
        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(SwaggerSecuritySchemes.ClientCredentials, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Client credentials flow — used for Book CRUD endpoints.",
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                [BookstoreScopes.BooksManage] = "Create, read, update, and delete books"
                            }
                        }
                    }
                });

                options.AddSecurityDefinition(SwaggerSecuritySchemes.Implicit, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Implicit flow — used for the Book search endpoint. Requires interactive sign-in (development only).",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                [BookstoreScopes.BooksRead] = "Search books"
                            }
                        }
                    }
                });

                options.OperationFilter<BookstoreSecurityRequirementsOperationFilter>();
            });

            return services;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.OAuthClientId(BookstoreClients.SwaggerClientId);
                options.OAuthScopes(BookstoreScopes.BooksRead);
            });

            return app;
        }
    }
}
