using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Persistence.Data;
using ResourceTracker.Persistence.Repositories;

namespace ResourceTracker.Persistence
{
    public static class PersistenceServicesResgistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureContext)
        {
            services.AddDbContext<ResourceTrackerDbContext>(configureContext);
            services.AddScoped<IResourceTrackerRepository, ResourceTrackerRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static Action<DbContextOptionsBuilder> ConfigureDbContext(string connectionstring)
        {
            return (DbContextOptionsBuilder options) =>
            {
                options.UseSqlServer(connectionstring);
            };
        }
    }
}
