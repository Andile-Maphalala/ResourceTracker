using EntitySecurity.Logic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ResourceTracker.Application;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Auth.Register;
using ResourceTracker.ImageStorageService;
using ResourceTracker.Persistence;
using ResourceTracker.Persistence.Data;
using Testcontainers.PostgreSql;

namespace ResourceTracker.IntegrationTests.Setup
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        public PostgreSqlContainer DbContainer { get; private set; }
        public IServiceScopeFactory ScopeFactory { get; private set; }

        public virtual async ValueTask InitializeAsync()
        {
            DbContainer = new PostgreSqlBuilder()
                 .WithImage("postgres:latest")
                 .WithDatabase("testdb")
                 .WithUsername("postgres")
                 .WithPassword("postgres")
                 .WithCleanUp(true)
                 .Build();

            await DbContainer.StartAsync();

            var services = new ServiceCollection();
            ConfigureServices(services);
            Mock<IUserInfo> UserInfoMock = new Mock<IUserInfo>();
            services.AddSingleton(UserInfoMock.Object);
            services.AddSingleton(UserInfoMock);
            var serviceProvider = services.BuildServiceProvider();

            ScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>();
            await db.Database.MigrateAsync();

            await SeedUsersAsync(scope);
        }

        public async ValueTask DisposeAsync()
        {
            await DbContainer.DisposeAsync();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["ConnectionString"] = DbContainer.GetConnectionString(),
            ["ApplicationOptions:BaseUrl"] = "http://localhost:5000"
        })
        .Build();
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            services.ConfigureApplicationServices(configuration);
            services.ConfigureImageStorageServices(Path.GetTempPath());
            services.AddEntitySecurity();

            services.ConfigurePersistenceServices((options) =>
            {
                options.UseNpgsql(DbContainer.GetConnectionString());
            }, configuration);
        }
        

        private async Task SeedUsersAsync(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var adminEmail = "admin@test.com";
            var adminUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == adminEmail);

            if (adminUser == null)
            {
                var command = new RegisterRequest
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Password = "Admin@123"
                };

                await sender.Send(command);
            }

            var userEmail = "user@test.com";
            var normalUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

            if (normalUser == null)
            {
                var command = new RegisterRequest
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "Normal",
                    LastName = "User",
                    Password = "User@123"
                };

                await sender.Send(command);
            }
        }
    }
}
