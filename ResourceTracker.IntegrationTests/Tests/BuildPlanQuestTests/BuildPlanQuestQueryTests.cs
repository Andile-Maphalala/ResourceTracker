using FluentAssertions;
using ResourceTracker.Application.Features.Queries.BuildPlanQuestQueries.GetBuildPlanQuestList;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanQuestTests
{
    public class BuildPlanQuestQueryTests : IntegrationTestBase
    {
        private int _gameId;
        private int _gameSaveId;
        private int _buildPlanId;
        private int _questAId;
        private int _questBId;

        public BuildPlanQuestQueryTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            _gameId = await AddGameRecord();
            _gameSaveId = await AddGameSaveRecord(_gameId);
            _buildPlanId = await AddBuildPlanRecord(_gameSaveId);
            _questAId = await AddQuestRecord(_gameSaveId);
            _questBId = await AddQuestRecord(_gameSaveId);

            DbContext.BuildPlanQuests.Add(new BuildPlanQuest { BuildPlanId = _buildPlanId, QuestId = _questAId });
            DbContext.BuildPlanQuests.Add(new BuildPlanQuest { BuildPlanId = _buildPlanId, QuestId = _questBId });

            var otherBp = new BuildPlan { Name = "Other BP", Description = "Other", GameSaveId = _gameSaveId };
            DbContext.BuildPlans.Add(otherBp);
            await DbContext.SaveChangesAsync();
            DbContext.BuildPlanQuests.Add(new BuildPlanQuest { BuildPlanId = otherBp.Id, QuestId = _questAId });
            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetBuildPlanQuestListQuery_WithExistingBuildPlanId_ReturnsList()
        {
            // Arrange
            SetupNonAdminUser();
            var query = new GetBuildPlanQuestListQuery(_buildPlanId, null);

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.QuestId).Should().Contain(new[] { _questAId, _questBId });
        }

        [Fact]
        public async Task GetBuildPlanQuestListQuery_WithExistingQuestId_ReturnsList()
        {
            // Arrange
            SetupNonAdminUser();
            var query = new GetBuildPlanQuestListQuery(null, _questAId);

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Count().Should().BeGreaterThanOrEqualTo(1);
            result.Select(r => r.QuestId).All(id => id == _questAId).Should().BeTrue();
        }

        [Fact]
        public async Task GetBuildPlanQuestListQuery_WhenNoFilters_EmptyList()
        {
            // Arrange
            SetupNonAdminUser();
            var query = new GetBuildPlanQuestListQuery(null, null);

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
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
