using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Common.Behavior;
using System.Reflection;

namespace ResourceTracker.Application
{
    public static class ApplicationServicesResgistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
