using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Persistence.Data;


namespace ResourceTracker.IntegrationTests.Setup
{
    [Collection("Integration")]
    public abstract class IntegrationTestBase
    {
        protected readonly IntegrationTestFixture Fixture;
        protected readonly IServiceScope Scope;
        protected readonly ISender Sender;
        protected readonly ResourceTrackerDbContext DbContext;
        protected Mock<IUserInfo> UserInfoMock;
        public readonly int AdminUserId = 1;
        public readonly int NonAdminUserId = 2;
        public readonly int IncorrectValue = 2147483647;
        public IntegrationTestBase(IntegrationTestFixture fixture)
        {
            Fixture = fixture;
            Scope = fixture.ScopeFactory.CreateScope();

            Sender = Scope.ServiceProvider.GetRequiredService<ISender>();
            DbContext = Scope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>();
            UserInfoMock = Scope.ServiceProvider.GetRequiredService<Mock<IUserInfo>>();

            ResetAsync(DbContext).GetAwaiter().GetResult();
            ClassSetup().GetAwaiter().GetResult();
        }

        protected virtual Task ClassSetup() => Task.CompletedTask;

        private static async Task ResetAsync(ResourceTrackerDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                "BuildPlanQuest",
                "BuildPlanComponent",
                "BuildPlan",
                "QuestComponents",
                "Recipe",
                "Component",
                "Quest",
                "GameSave",
                "Game",
                "Picture"

            RESTART IDENTITY CASCADE;
        """);
        }

        public void SetupNonAdminUser()
        {
            SetUser(NonAdminUserId,false);
        }

        public void SetupAdminUser()
        {
            SetUser(AdminUserId, true);
        }

        public void SetUser(int userId, bool isAdmin = false)
        {
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(isAdmin);
            UserInfoMock
            .Setup(x => x.GetUserId())
            .Returns(userId);
        }
    }

}
