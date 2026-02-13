using FluentAssertions;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.QuestTests
{
    public class QuestQueryTests : IntegrationTestBase
    {
        private int _subnauticaGameId;
        private int _coralReefSaveId;

        public QuestQueryTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            DbContext.Games.AddRange(
                new Game { Name = "Need for Speed", Description = "Arcade racing game" },
                new Game { Name = "Subnautica", Description = "Underwater survival exploration" },
                new Game { Name = "Minecraft", Description = "Sandbox survival crafting" }
            );

            await DbContext.SaveChangesAsync();

            Game subnautica = DbContext.Games.First(g => g.Name == "Subnautica");
            _subnauticaGameId = subnautica.Id;

            DbContext.GameSaves.AddRange(
                new GameSave { GameId = subnautica.Id, Name = "Coral Reef Base", Description = "Save1", UserId = AdminUserId },
                new GameSave { GameId = subnautica.Id, Name = "Deep Sea Exploration", Description = "Save2", UserId = AdminUserId }
            );

            await DbContext.SaveChangesAsync();

            GameSave coral = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base");
            _coralReefSaveId = coral.Id;

            DbContext.Quests.AddRange(
                new Quest
                {
                    Name = "Find the Lost Cache",
                    Description = "Beginner quest near the reef",
                    Location = "Shallow Reef",
                    GameSaveId = coral.Id
                },
                new Quest
                {
                    Name = "Deep Dive",
                    Description = "Advanced exploration of the abyss",
                    Location = "Abyssal Trench",
                    GameSaveId = coral.Id
                },
                new Quest
                {
                    Name = "Surface Salvage",
                    Description = "Collect floating debris on the surface",
                    Location = "Open Water",
                    GameSaveId = DbContext.GameSaves.First(gs => gs.Name == "Deep Sea Exploration").Id
                },
                new Quest
                {
                    Name = "T4",
                    Description = "watering",
                    Location = "T4",
                    GameSaveId = DbContext.GameSaves.First(gs => gs.Name == "Deep Sea Exploration").Id
                }
            );

            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetQuest_WhenFound_ReturnsQuestWithAllFields()
        {
            Quest existing = DbContext.Quests.First(q => q.Name == "Find the Lost Cache");
            GetQuestQuery query = new GetQuestQuery(existing.Id);

            GetQuestResponse result = await Sender.Send(query, CancellationToken.None);

            result.Id.Should().Be(existing.Id);
            result.Name.Should().Be(existing.Name);
            result.Description.Should().Be(existing.Description);
            result.Location.Should().Be(existing.Location);
            result.GameSaveId.Should().Be(existing.GameSaveId);
        }

        [Fact]
        public async Task GetQuest_WhenNotFound_ThrowsNotFoundException()
        {
            GetQuestQuery query = new GetQuestQuery(999999);

            NotFoundException result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (Quest) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task Search_NoFilters_ReturnsAllQuests()
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().HaveCount(4);
            result.Data.Select(x => x.Name)
                .Should().Contain(new[] { "Find the Lost Cache", "Deep Dive", "Surface Salvage","T4" });
        }

        [Theory]
        [InlineData("Find", "Find the Lost Cache")]
        [InlineData("Deep", "Deep Dive")]
        [InlineData("Surface", "Surface Salvage")]
        public async Task Search_NameStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                Name = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("Beginner", "Find the Lost Cache")]
        [InlineData("Advanced", "Deep Dive")]
        [InlineData("Collect", "Surface Salvage")]
        public async Task Search_DescriptionStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                Description = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("reef", new[] { "Find the Lost Cache" })]
        [InlineData("water", new[] { "Surface Salvage","T4" })]
        public async Task Search_SearchTermContains_ReturnsMatchingQuests(string queryText, string[] expectedNames)
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Select(x => x.Name).Should().Contain(expectedNames);
        }

        [Fact]
        public async Task Search_IsCaseInsensitive()
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                SearchTerms = "SHALLOW REEF",
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be("Find the Lost Cache");
        }

        [Fact]
        public async Task Search_WithGameSaveId_ReturnsOnlyQuestsForThatGameSave()
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                GameSaveId = _coralReefSaveId,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().HaveCount(2);
            result.Data.Select(q => q.Name).Should().Contain(new[] { "Find the Lost Cache", "Deep Dive" });
        }

        [Fact]
        public async Task Search_ReturnedItemsContainAllFields()
        {
            SearchQuestsQuery query = new SearchQuestsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchQuestsResponse> result = await Sender.Send(query, CancellationToken.None);

            SearchQuestsResponse item = result.Data.First(x => x.Name == "Find the Lost Cache");

            item.Id.Should().BeGreaterThan(0);
            item.Name.Should().NotBeNullOrWhiteSpace();
            item.Description.Should().NotBeNullOrWhiteSpace();
            item.Location.Should().NotBeNullOrWhiteSpace();
            item.GameSaveId.Should().Be(_coralReefSaveId);
            item.GameId.Should().Be(_subnauticaGameId);
            item.GameName.Should().Be("Subnautica");
            item.GameSaveName.Should().Be("Coral Reef Base");
        }
    }
}