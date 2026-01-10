
using EntitySecurity.Contract.Security;
using EntitySecurity.Logic;
using EntitySecurity.Logic.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ResourceTracker.Application;
using ResourceTracker.ImageStorageService;
using ResourceTracker.Persistence;
using ResourceTracker.Persistence.Data;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ResourceTracker.IntegrationTests
{
    public class IntegrationTestBase : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        protected IServiceScope _serviceScope { get; private set; }
        protected ResourceTrackerDbContext _dbContext { get; private set; }
        protected IServiceProvider _serviceProvider;
        protected WebApplicationFactory<Program> _factory;
        protected HttpClient HttpClient { get; private set; }
        protected IServiceScope _scope;

        public IntegrationTestBase()
        {
            _dbContainer = new PostgreSqlBuilder()
                 .WithImage("postgres:latest")
                 .WithDatabase("testdb")
                 .WithUsername("postgres")
                 .WithPassword("postgres")
                 .WithCleanUp(true)
                 .Build();
        }
        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionString"] = _dbContainer.GetConnectionString()
                }).Build();

            services.ConfigureApplicationServices();
            services.ConfigureImageStorageServices(Path.GetTempPath());
            services.AddEntitySecurity();
            services.AddScoped<IInfoSetter, InfoSetter>();

            services.ConfigurePersistenceServices((DbContextOptionsBuilder options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            }, configuration);


            var serviceProvider = services.BuildServiceProvider();

            _serviceScope = serviceProvider.CreateScope();
            _dbContext = _serviceScope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>();
            _serviceProvider = _serviceScope.ServiceProvider;

            await _dbContext.Database.MigrateAsync();
        }
        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            _serviceScope?.Dispose();
            await _dbContainer.DisposeAsync();
        }


    }

}
