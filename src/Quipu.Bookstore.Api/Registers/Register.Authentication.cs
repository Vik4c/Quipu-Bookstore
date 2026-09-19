using Quipu.Bookstore.Api.Authentication.Seeding;

using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Validation.AspNetCore;

namespace Quipu.Bookstore.Api.Registers
{
    public static partial class Register
    {
        public static IServiceCollection RegisterAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DevelopmentAuthOptions>(configuration.GetSection(DevelopmentAuthOptions.SectionName));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => options.LoginPath = "/connect/login");

            return services;
        }
    }
}
