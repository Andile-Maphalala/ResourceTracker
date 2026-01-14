using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.GameSaveTests
{
    public class GameSaveQueryTests : IntegrationTestBase
    {
        private int _subnauticaGameId;
        public GameSaveQueryTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            SetupNonAdminUser();

            DbContext.Games.AddRange(
                 new Game { Name = "Need for Speed", Description = "Arcade racing game" },
                 new Game { Name = "Gran Turismo", Description = "Simulation racing" },
                 new Game { Name = "Speed Runner", Description = "Fast paced platformer" },
                 new Game { Name = "Doom", Description = "Fast action shooter" },
                 new Game { Name = "Subnautica", Description = "Underwater survival exploration" },
                 new Game { Name = "Minecraft", Description = "Sandbox survival crafting" }
             );

            await DbContext.SaveChangesAsync();

            var subnautica = DbContext.Games.First(g => g.Name == "Subnautica");
            _subnauticaGameId = subnautica.Id;
            DbContext.GameSaves.AddRange(
                new GameSave { GameId = subnautica.Id, Name = "Coral Reef Base", Description = "Save1", UserId = UserInfoMock.Object.GetUserId()},
                new GameSave { GameId = subnautica.Id, Name = "Deep Sea Exploration", Description = "Save2" , UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = subnautica.Id, Name = "Playing Around", Description = "Save3" , UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = subnautica.Id, Name = "First", Description = "Save1", UserId = AdminUserId }
            );

            var minecraft = DbContext.Games.First(g => g.Name == "Minecraft");
            DbContext.GameSaves.AddRange(
                new GameSave { GameId = minecraft.Id, Name = "Survival World", Description = "SaveA", UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = minecraft.Id, Name = "Creative Build", Description = "SaveB", UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = minecraft.Id, Name = "Adventure Map", Description = "SaveC", UserId = AdminUserId }
            );
            await DbContext.SaveChangesAsync();
        }



        [Fact]
        public async Task GetGameSave_CorrectIdCorrectUser_ReturnGame()
        {
            // Arrange
            SetupNonAdminUser();
            var existing = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base");
            var query = new GetGameSaveQuery { Id = existing.Id };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Id.Should().Be(existing.Id);
            result.Name.Should().Be(existing.Name);
            result.Description.Should().Be(existing.Description);
            result.Created.Should().Be(existing.Created);
            result.GameId.Should().Be(existing.GameId);
            result.GameName.Should().Be("Subnautica");
        }

        [Fact]
        public async Task GetGameSave_InCorrectIdCorrectUser_ThrowsError()
        {
            // Arrange
            SetupNonAdminUser();
            var query = new GetGameSaveQuery { Id = IncorrectValue };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (Game Save) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task GetGameSave_CorrectIdInCorrectUser_ThrowsError()
        {
            // Arrange
            SetUser(IncorrectValue);
            var existing = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base");
            var query = new GetGameSaveQuery { Id = existing.Id };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (Game Save) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task GetGameSaveListQuery_WithExistingGameId_ReturnsUserGameSaveList()
        {
            // Arrange
            SetupNonAdminUser();
            var query = new GetGameSaveListQuery { GameId = _subnauticaGameId };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Select(gs => gs.Name).Should().Contain(new[] { "Coral Reef Base", "Deep Sea Exploration", "Playing Around" });
        }

        [Fact]
        public async Task GetGameSaveListQuery_WhenGameNotFound_EmptyList()
        {
            // Arrange
            SetupNonAdminUser();
            var query = new GetGameSaveListQuery { GameId = IncorrectValue };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

    }
}