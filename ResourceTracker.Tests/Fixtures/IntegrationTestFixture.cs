
using EntitySecurity.Contract.Security;
using EntitySecurity.Logic;
using EntitySecurity.Logic.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Persistence.Data;
using Testcontainers.PostgreSql;
using ResourceTracker.Persistence;
using ResourceTracker.Application;
using ResourceTracker.ImageStorageService;
namespace ResourceTracker.Tests.Fixtures
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private PostgreSqlContainer _dbContainer;
        protected readonly IServiceScope Scope;
        protected readonly ResourceTrackerDbContext DbContext;
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public async Task InitializeAsync()
        {
            _dbContainer = new PostgreSqlBuilder()
             .WithImage("postgres:latest")
             .WithDatabase("testdb")
             .WithUsername("postgres")
             .WithPassword("postgres")
             .WithCleanUp(true)
             .Build();

            await _dbContainer.StartAsync();

            // Build configuration
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionString"] = _dbContainer.GetConnectionString()
                }).Build();

            // Setup DI container
            var services = new ServiceCollection();

            // Register your services exactly like in Program.cs
            services.ConfigureApplicationServices();
            services.ConfigureImageStorageServices();
            services.AddEntitySecurity();
            services.AddScoped<IInfoSetter, InfoSetter>();

            services.ConfigurePersistenceServices((DbContextOptionsBuilder options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            }, Configuration);

            ServiceProvider = services.BuildServiceProvider();

            // Apply migrations
            await ApplyDbMigrationsAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }

        private async Task ApplyDbMigrationsAsync()
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>();

            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        public T GetService<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        public IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }

    }
}
