using FluentAssertions;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.GetBuildPlan;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.SearchBuildPlans;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanTests
{
    public class BuildPlanQueriesTests : IntegrationTestBase
    {
        private int _subnauticaGameId;
        private int _coralReefSaveId;

        public BuildPlanQueriesTests(IntegrationTestFixture fixture) : base(fixture)
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

            DbContext.BuildPlans.AddRange(
                new BuildPlan
                {
                    Name = "Alpha Build",
                    Description = "Beginner build near the reef",
                    GameSaveId = coral.Id
                },
                new BuildPlan
                {
                    Name = "Deep Build",
                    Description = "Advanced deep build",
                    GameSaveId = coral.Id
                },
                new BuildPlan
                {
                    Name = "Surface Build",
                    Description = "Collect floating debris on the surface",
                    GameSaveId = DbContext.GameSaves.First(gs => gs.Name == "Deep Sea Exploration").Id
                },
                new BuildPlan
                {
                    Name = "T4",
                    Description = "watering",
                    GameSaveId = DbContext.GameSaves.First(gs => gs.Name == "Deep Sea Exploration").Id
                }
            );

            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetBuildPlan_WhenFound_ReturnsBuildPlanWithAllFields()
        {
            BuildPlan existing = DbContext.BuildPlans.First(q => q.Name == "Alpha Build");
            GetBuildPlanQuery query = new GetBuildPlanQuery(existing.Id);

            GetBuildPlanResponse result = await Sender.Send(query, CancellationToken.None);

            result.Id.Should().Be(existing.Id);
            result.Name.Should().Be(existing.Name);
            result.Description.Should().Be(existing.Description);
            result.GameSaveId.Should().Be(existing.GameSaveId);
            result.GameSaveName.Should().Be(DbContext.GameSaves.First(gs => gs.Id == existing.GameSaveId).Name);
        }

        [Fact]
        public async Task GetBuildPlan_WhenNotFound_ThrowsNotFoundException()
        {
            GetBuildPlanQuery query = new GetBuildPlanQuery(999999);

            NotFoundException result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlan) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task Search_NoFilters_ReturnsAllBuildPlans()
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().HaveCount(4);
            result.Data.Select(x => x.Name)
                .Should().Contain(new[] { "Alpha Build", "Deep Build", "Surface Build", "T4" });
        }

        [Theory]
        [InlineData("Alpha", "Alpha Build")]
        [InlineData("Deep", "Deep Build")]
        [InlineData("Surface", "Surface Build")]
        public async Task Search_NameStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                Name = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("Beginner", "Alpha Build")]
        [InlineData("Advanced", "Deep Build")]
        [InlineData("Collect", "Surface Build")]
        public async Task Search_DescriptionStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                Description = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("reef", new[] { "Alpha Build" })]
        [InlineData("surface", new[] { "Surface Build"})]
        public async Task Search_SearchTermContains_ReturnsMatchingBuildPlans(string queryText, string[] expectedNames)
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Select(x => x.Name).Should().Contain(expectedNames);
        }

        [Fact]
        public async Task Search_IsCaseInsensitive()
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                SearchTerms = "THE REEF",
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be("Alpha Build");
        }

        [Fact]
        public async Task Search_WithGameSaveId_ReturnsOnlyBuildPlansForThatGameSave()
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                GameSaveId = _coralReefSaveId,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().HaveCount(2);
            result.Data.Select(q => q.Name).Should().Contain(new[] { "Alpha Build", "Deep Build" });
        }

        [Fact]
        public async Task Search_ReturnedItemsContainAllFields()
        {
            SearchBuildPlansQuery query = new SearchBuildPlansQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchBuildPlansResponse> result = await Sender.Send(query, CancellationToken.None);

            SearchBuildPlansResponse item = result.Data.First(x => x.Name == "Alpha Build");

            item.Id.Should().BeGreaterThan(0);
            item.Name.Should().NotBeNullOrWhiteSpace();
            item.Description.Should().NotBeNullOrWhiteSpace();
            item.GameSaveId.Should().Be(_coralReefSaveId);
            item.GameId.Should().Be(_subnauticaGameId);
            item.GameName.Should().Be("Subnautica");
            item.GameSaveName.Should().Be("Coral Reef Base");
        }
    }
}
