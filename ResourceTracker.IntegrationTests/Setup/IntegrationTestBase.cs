
using EntitySecurity.Contract.Security;
using EntitySecurity.Logic;
using EntitySecurity.Logic.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using ResourceTracker.Application;
using ResourceTracker.Application.Common.User;
using ResourceTracker.ImageStorageService;
using ResourceTracker.Persistence;
using ResourceTracker.Persistence.Data;
using Testcontainers.PostgreSql;

namespace ResourceTracker.IntegrationTests.Setup
{
    public class IntegrationTestBase : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;
        protected IServiceScope _serviceScope { get; private set; }
        protected ResourceTrackerDbContext _dbContext { get; private set; }
        protected WebApplicationFactory<Program> _factory;
        protected HttpClient HttpClient { get; private set; }
        public Mock<IUserInfo> UserInfoMock { get; } = new();
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
        public virtual async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _serviceScope = serviceProvider.CreateScope();
            _dbContext = _serviceScope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>();

            await _dbContext.Database.MigrateAsync();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>
               {
                   ["ConnectionString"] = _dbContainer.GetConnectionString()
               }).Build();

            services.ConfigureApplicationServices();
            services.ConfigureImageStorageServices(Path.GetTempPath());
            services.AddEntitySecurity();
            services.AddScoped<IInfoSetter, InfoSetter>();

            services.ConfigurePersistenceServices((options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            }, configuration);

            UserInfoMock.Setup(x => x.IsAdmin()).Returns(false);
            services.AddSingleton(UserInfoMock.Object);

        }
        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            _serviceScope?.Dispose();
            await _dbContainer.DisposeAsync();
        }
    }

}
