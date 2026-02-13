using FluentAssertions;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.QuestComponentTests
{
    public class QuestComponentQueryTests : IntegrationTestBase
    {
        public QuestComponentQueryTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            // Seed games
            DbContext.Games.AddRange(
                new Game { Name = "Subnautica", Description = "Underwater survival exploration" },
                new Game { Name = "Minecraft", Description = "Sandbox survival crafting" }
            );
            await DbContext.SaveChangesAsync();

            // Seed game saves
            var subnautica = DbContext.Games.First(g => g.Name == "Subnautica");
            DbContext.GameSaves.AddRange(
                new GameSave { GameId = subnautica.Id, Name = "Coral Reef Base", Description = "Save1", UserId = AdminUserId },
                new GameSave { GameId = subnautica.Id, Name = "Deep Sea Exploration", Description = "Save2", UserId = AdminUserId }
            );
            await DbContext.SaveChangesAsync();

            // Seed quests
            var coral = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base");
            var deep = DbContext.GameSaves.First(gs => gs.Name == "Deep Sea Exploration");
            DbContext.Quests.AddRange(
                new Quest { Name = "Find the Lost Cache", Description = "Beginner quest near the reef", Location = "Shallow Reef", GameSaveId = coral.Id },
                new Quest { Name = "Deep Dive", Description = "Advanced exploration of the abyss", Location = "Abyssal Trench", GameSaveId = coral.Id },
                new Quest { Name = "Surface Salvage", Description = "Collect floating debris on the surface", Location = "Open Water", GameSaveId = deep.Id }
            );
            await DbContext.SaveChangesAsync();

            // Seed components
            var game = DbContext.Games.First(g => g.Name == "Subnautica");
            DbContext.Components.AddRange(
                new Component { Name = "Copper Ore", Description = "Basic ore", GameId = game.Id, Type = 1 },
                new Component { Name = "Iron Ingot", Description = "Smelt into bars", GameId = game.Id, Type = 1 },
                new Component { Name = "Conveyor Belt", Description = "Transports items", GameId = game.Id, Type = 2 }
            );
            await DbContext.SaveChangesAsync();

            // Seed quest components (links)
            var questFind = DbContext.Quests.First(q => q.Name == "Find the Lost Cache");
            var questDeep = DbContext.Quests.First(q => q.Name == "Deep Dive");
            var copper = DbContext.Components.First(c => c.Name == "Copper Ore");
            var iron = DbContext.Components.First(c => c.Name == "Iron Ingot");
            var conveyor = DbContext.Components.First(c => c.Name == "Conveyor Belt");

            DbContext.QuestComponents.AddRange(
                new QuestComponents { QuestId = questFind.Id, ComponentId = copper.Id, AmountAquired = 3 },
                new QuestComponents { QuestId = questFind.Id, ComponentId = iron.Id, AmountAquired = 1 },
                new QuestComponents { QuestId = questDeep.Id, ComponentId = conveyor.Id, AmountAquired = 5 }
            );
            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetQuestComponent_WhenFound_ReturnsQuestComponent()
        {
            // Arrange
            var existing = DbContext.QuestComponents.First();
            var query = new GetQuestComponentQuery(existing.Id);

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Id.Should().Be(existing.Id);
            result.AmountAquired.Should().Be(existing.AmountAquired);
            result.ComponentName.Should().Be(existing.Component.Name);
            result.ComponentType.Should().Be(existing.Component.Type);
            result.QuestId.Should().Be(existing.QuestId);
            result.QuestName.Should().Be(existing.Quest.Name);
        }

        [Fact]
        public async Task GetQuestComponent_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var query = new GetQuestComponentQuery(999999);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (Quest component) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task Search_NoFilters_ReturnsAll()
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(DbContext.QuestComponents.Count());
        }

        [Theory]
        [InlineData("Copper", "Copper Ore")]
        [InlineData("Conveyor", "Conveyor Belt")]
        public async Task Search_ComponentNameStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                ComponentName = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("smelt", "Iron Ingot")]
        [InlineData("transports", "Conveyor Belt")]
        public async Task Search_ComponentDescriptionStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                ComponentDescription = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be(expectedName);
        }

        [Fact]
        public async Task Search_WithType_ReturnsOnlyThatType()
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                Type = 2,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be("Conveyor Belt");
        }

        [Fact]
        public async Task Search_WithComponentId_ReturnsSingleRecord()
        {
            // Arrange
            var component = DbContext.Components.First(c => c.Name == "Copper Ore");
            var query = new SearchQuestComponentsQuery
            {
                ComponentId = component.Id,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be("Copper Ore");
        }

        [Fact]
        public async Task Search_WithQuestId_ReturnsOnlyComponentsForThatQuest()
        {
            // Arrange
            var quest = DbContext.Quests.First(q => q.Name == "Find the Lost Cache");
            var query = new SearchQuestComponentsQuery
            {
                QuestId = quest.Id,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(2);
            result.Data.Select(x => x.ComponentName).Should().Contain(new[] { "Copper Ore", "Iron Ingot" });
        }

        [Fact]
        public async Task Search_QuestNameStartsWith_ReturnRecord()
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                QuestName = "Find",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(2);
            result.Data.All(x => x.QuestName.StartsWith("Find")).Should().BeTrue();
        }

        [Fact]
        public async Task Search_IdFilter_ReturnsSingle()
        {
            // Arrange
            var existing = DbContext.QuestComponents.First();
            var query = new SearchQuestComponentsQuery
            {
                Id = existing.Id,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().Id.Should().Be(existing.Id);
        }

        [Theory]
        [InlineData("lost", new[] { "Find the Lost Cache" })]
        [InlineData("conveyor", new[] { "Deep Dive" })]
        public async Task Search_SearchTermsContains_ReturnsMatchingEntries(string queryText, string[] expectedQuestNames)
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Select(x => x.QuestName).Should().Contain(expectedQuestNames);
        }

        [Fact]
        public async Task Search_IsCaseInsensitive()
        {
            // Arrange
            var query = new SearchQuestComponentsQuery
            {
                SearchTerms = "COPPER",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchQuestComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be("Copper Ore");
        }
    }
}