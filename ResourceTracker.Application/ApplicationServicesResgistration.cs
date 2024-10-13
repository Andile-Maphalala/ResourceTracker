using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Common.Behavior;
using System.Reflection;
using FluentValidation;


namespace ResourceTracker.Application
{
    public static class ApplicationServicesResgistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorWithIRequest<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorWithIRequestAndIResponse<,>));

            return services;
        }
    }
}
