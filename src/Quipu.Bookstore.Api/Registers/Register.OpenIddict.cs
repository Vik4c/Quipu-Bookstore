using Quipu.Bookstore.Api.Authentication;
using Quipu.Bookstore.Api.Authorization;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace Quipu.Bookstore.Api.Registers
{
    public static partial class Register
    {
        public static IServiceCollection RegisterOpenIddict(
            this IServiceCollection services,
            IWebHostEnvironment environment)
        {
            services
                .AddOpenIddict()
                .AddServer(options =>
                {
                    options
                        .SetTokenEndpointUris("connect/token")
                        .SetAuthorizationEndpointUris("connect/authorize");

                    options.RegisterScopes(BookstoreScopes.BooksManage, BookstoreScopes.BooksRead);

                    options
                        .AllowClientCredentialsFlow()
                        .AllowImplicitFlow();

                    options.AddEventHandler<HandleTokenRequestContext>(builder =>
                        builder.UseSingletonHandler<ClientCredentialsTokenHandler>());

                    if (environment.IsDevelopment())
                        options
                            .AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();

                    OpenIddictServerAspNetCoreBuilder aspNetCore = options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough();

                    if (environment.IsDevelopment())
                        aspNetCore.DisableTransportSecurityRequirement();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.AddAudiences(BookstoreAudience.Api);
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}
