namespace Quipu.Bookstore.Api.Registers
{
    public static partial class Register
    {
        public static IServiceCollection RegisterControllers(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddProblemDetails();

            return services;
        }
    }
}
