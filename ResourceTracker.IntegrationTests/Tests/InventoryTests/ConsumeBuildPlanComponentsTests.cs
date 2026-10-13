using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Commands.InventoryCommands.ConsumeBuildPlanComponents;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.InventoryTests
{
    public class ConsumeBuildPlanComponentsTests : IntegrationTestBase
    {
        private int _gameId;
        private int _gameSaveId;
        private int _questId;

        public ConsumeBuildPlanComponentsTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            SetupNonAdminUser();

            _gameId = await AddGameRecord();
            _gameSaveId = await AddGameSaveRecord(_gameId);
            _questId = await AddQuestRecord(_gameSaveId);
        }

        [Fact]
        public async Task Consume_StockInInventory_DeductsTheAmount()
        {
            var componentId = await AddComponentRecord("Iron", ComponentTypeEnum.Resource);
            var questComponentId = await AddQuestComponentRecord(componentId, 10);

            var result = await Sender.Send(new ConsumeBuildPlanComponentsCommand(new List<ConsumeAllocation>
            {
                new ConsumeAllocation { QuestComponentId = questComponentId, AmountToDeduct = 4 }
            }), CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            await QuantityShouldBe(questComponentId, 6);
        }

        [Fact]
        public async Task Consume_FacilityRow_IsRefusedAndLeftUntouched()
        {
            // A facility is capability the player owns, not stock, so it is never consumed. It is
            // refused rather than silently skipped, and the whole request fails so that nothing
            // is half-applied.
            var componentId = await AddComponentRecord("Fabricator", ComponentTypeEnum.Facility);
            var questComponentId = await AddQuestComponentRecord(componentId, 1);

            var result = await Sender.Send(new ConsumeBuildPlanComponentsCommand(new List<ConsumeAllocation>
            {
                new ConsumeAllocation { QuestComponentId = questComponentId, AmountToDeduct = 1 }
            }), CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.Single().Should().Contain("Fabricator");
            await QuantityShouldBe(questComponentId, 1);
        }

        [Fact]
        public async Task Consume_FacilityAlongsideStock_AppliesNeither()
        {
            // The facility error must not let the stock deduction through: an all-or-nothing request
            // is what keeps the inventory consistent when the client sends both together.
            var ironId = await AddComponentRecord("Iron", ComponentTypeEnum.Resource);
            var facilityId = await AddComponentRecord("Fabricator", ComponentTypeEnum.Facility);
            var ironRow = await AddQuestComponentRecord(ironId, 10);
            var facilityRow = await AddQuestComponentRecord(facilityId, 1);

            var result = await Sender.Send(new ConsumeBuildPlanComponentsCommand(new List<ConsumeAllocation>
            {
                new ConsumeAllocation { QuestComponentId = ironRow, AmountToDeduct = 4 },
                new ConsumeAllocation { QuestComponentId = facilityRow, AmountToDeduct = 1 }
            }), CancellationToken.None);

            result.Success.Should().BeFalse();
            await QuantityShouldBe(ironRow, 10);
            await QuantityShouldBe(facilityRow, 1);
        }

        [Fact]
        public async Task Consume_MoreThanStock_ReportsShortfallAndAppliesNothing()
        {
            var componentId = await AddComponentRecord("Iron", ComponentTypeEnum.Resource);
            var questComponentId = await AddQuestComponentRecord(componentId, 2);

            var result = await Sender.Send(new ConsumeBuildPlanComponentsCommand(new List<ConsumeAllocation>
            {
                new ConsumeAllocation { QuestComponentId = questComponentId, AmountToDeduct = 5 }
            }), CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Errors.Single().Should().Contain("Insufficient stock");
            await QuantityShouldBe(questComponentId, 2);
        }

        private async Task QuantityShouldBe(int questComponentId, int expected)
        {
            var amount = await DbContext.QuestComponents
                .AsNoTracking()
                .Where(qc => qc.Id == questComponentId)
                .Select(qc => qc.AmountAquired)
                .SingleAsync();

            amount.Should().Be(expected);
        }

        private async Task<int> AddGameRecord()
        {
            var game = new Game
            {
                Name = "Consume Test Game",
                Description = "Consume tests"
            };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

        private async Task<int> AddGameSaveRecord(int gameId)
        {
            var gameSave = new GameSave
            {
                Name = "Consume Test Save",
                Description = "Consume tests",
                GameId = gameId,
                Created = DateTime.UtcNow,
                UserId = NonAdminUserId
            };
            DbContext.GameSaves.Add(gameSave);
            await DbContext.SaveChangesAsync();
            return gameSave.Id;
        }

        private async Task<int> AddQuestRecord(int gameSaveId)
        {
            var quest = new Quest
            {
                Name = "Consume Test Quest",
                Description = "Consume tests",
                Location = "Consume Test Location",
                GameSaveId = gameSaveId
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            return quest.Id;
        }

        private async Task<int> AddComponentRecord(string name, ComponentTypeEnum type)
        {
            var component = new Component
            {
                Name = name,
                Description = "Consume tests",
                GameId = _gameId,
                Type = (int)type
            };
            DbContext.Components.Add(component);
            await DbContext.SaveChangesAsync();
            return component.Id;
        }

        private async Task<int> AddQuestComponentRecord(int componentId, int amount)
        {
            var questComponent = new QuestComponents
            {
                AmountAquired = amount,
                ComponentId = componentId,
                QuestId = _questId
            };
            DbContext.QuestComponents.Add(questComponent);
            await DbContext.SaveChangesAsync();
            return questComponent.Id;
        }
    }
}
