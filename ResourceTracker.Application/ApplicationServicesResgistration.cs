using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Common.Behavior;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Auth;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using System.Reflection;


namespace ResourceTracker.Application
{
    public static class ApplicationServicesResgistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblyContaining<CreateGameValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorWithIRequest<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorWithIRequestAndIResponse<,>));
            services.AddScoped<IUserInfo, UserInfo>();

            return services;
        }
    }
}
