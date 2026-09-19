using Quipu.Bookstore.Api.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Quipu.Bookstore.Api.Swagger
{
    public class BookstoreSecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            AuthorizeAttribute? authorize = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault();

            if (authorize?.Policy is null)
                return;

            (string schemeId, string scope) = authorize.Policy switch
            {
                BookstorePolicies.BooksManage => (SwaggerSecuritySchemes.ClientCredentials, BookstoreScopes.BooksManage),
                BookstorePolicies.BooksRead => (SwaggerSecuritySchemes.Implicit, BookstoreScopes.BooksRead),
                _ => throw new InvalidOperationException($"No Swagger security scheme is mapped for policy '{authorize.Policy}'.")
            };

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            OpenApiSecurityScheme schemeReference = new()
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = schemeId }
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [schemeReference] = [scope]
            });
        }
    }
}
