using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.CreateBuildPlanQuest;
using ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.DeleteBuildPlanQuest;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanQuestTests
{
    public class BuildPlanQuestCommandTests : IntegrationTestBase
    {
        private int gameId;
        private int gameSaveId;
        private int buildPlanId;
        private int questId;

        public BuildPlanQuestCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            gameId = await AddGameRecord();
            gameSaveId = await AddGameSaveRecord(gameId);
            buildPlanId = await AddBuildPlanRecord(gameSaveId);
            questId = await AddQuestRecord(gameSaveId);
        }

        [Fact]
        public async Task CreateBuildPlanQuest_BuildPlanIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanQuestCommand
            {
                BuildPlanId = 0,
                QuestId = questId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("BuildPlanId is required");
        }

        [Fact]
        public async Task CreateBuildPlanQuest_QuestIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanQuestCommand
            {
                BuildPlanId = buildPlanId,
                QuestId = 0
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("QuestId is required");
        }

        [Fact]
        public async Task CreateBuildPlanQuest_InvalidUser_ThrowError()
        {
            // Arrange
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            var command = new CreateBuildPlanQuestCommand
            {
                BuildPlanId = buildPlanId,
                QuestId = questId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task CreateBuildPlanQuest_ValidRequest_CreateRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanQuestCommand
            {
                BuildPlanId = buildPlanId,
                QuestId = questId
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var entity = await DbContext.BuildPlanQuests.FindAsync(new object[] { result.Id, questId });
            // The composite key may be stored as (BuildPlanId, QuestId), search accordingly
            var found = await DbContext.BuildPlanQuests.FirstOrDefaultAsync(x => x.BuildPlanId == result.Id && x.QuestId == questId);
            found.Should().NotBeNull();
            found!.BuildPlanId.Should().Be(command.BuildPlanId);
            found.QuestId.Should().Be(command.QuestId);
        }

        [Fact]
        public async Task DeleteBuildPlanQuest_NotFound_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanQuestCommand(IncorrectValue, IncorrectValue);

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlanQuest) with Key (BuildPlanId:{command.BuildPlanId}, QuestId:{command.QuestId}) was not found.");
        }

        [Fact]
        public async Task DeleteBuildPlanQuest_BuildPlanIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanQuestCommand(0, questId);

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("BuildPlanId is required");
        }

        [Fact]
        public async Task DeleteBuildPlanQuest_QuestIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanQuestCommand(buildPlanId, 0);

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("QuestId is required");
        }

        [Fact]
        public async Task DeleteBuildPlanQuest_ValidRequest_DeleteRecord()
        {
            // Arrange
            SetupNonAdminUser();
            // Add record
            var bpq = new BuildPlanQuest { BuildPlanId = buildPlanId, QuestId = questId };
            DbContext.BuildPlanQuests.Add(bpq);
            await DbContext.SaveChangesAsync();

            var command = new DeleteBuildPlanQuestCommand(buildPlanId, questId);

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var found = await DbContext.BuildPlanQuests.FirstOrDefaultAsync(x => x.BuildPlanId == buildPlanId && x.QuestId == questId);
            found.Should().BeNull();
        }

        #region Helpers
        private async Task<int> AddGameRecord()
        {
            var game = new Game
            {
                Name = "Existing Game",
                Description = "Existing Description"
            };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

        private async Task<int> AddGameSaveRecord(int gameId)
        {
            var gs = new GameSave
            {
                Name = "Existing Game Save",
                Description = "Existing Description",
                GameId = gameId,
                Created = DateTime.UtcNow,
                UserId = NonAdminUserId
            };
            DbContext.GameSaves.Add(gs);
            await DbContext.SaveChangesAsync();
            return gs.Id;
        }

        private async Task<int> AddBuildPlanRecord(int gameSaveId)
        {
            var bp = new BuildPlan
            {
                Name = "Existing BuildPlan",
                Description = "Existing Description",
                GameSaveId = gameSaveId
            };
            DbContext.BuildPlans.Add(bp);
            await DbContext.SaveChangesAsync();
            return bp.Id;
        }

        private async Task<int> AddQuestRecord(int gameSaveId)
        {
            var quest = new Quest
            {
                Name = "Existing Quest",
                Description = "Existing Description",
                Location = "Existing Location",
                GameSaveId = gameSaveId
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            return quest.Id;
        }
        #endregion
    }
}
