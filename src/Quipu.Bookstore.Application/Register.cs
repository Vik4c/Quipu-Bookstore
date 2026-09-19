using Quipu.Bookstore.Application.Abstractions;

using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Quipu.Bookstore.Application
{
    public static partial class Register
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Register))
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestWithResultHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            return services;
        }
    }
}
