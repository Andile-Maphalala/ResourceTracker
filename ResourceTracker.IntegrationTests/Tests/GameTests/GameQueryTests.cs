using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.GameTests
{
    public class GameQueryTests : IntegrationTestBase
    {
        public GameQueryTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            await SeedGames();
        }

        private async Task SeedGames()
        {
            DbContext.Games.AddRange(
                new Game { Name = "Need for Speed", Description = "Arcade racing game" },
                new Game { Name = "Gran Turismo", Description = "Simulation racing" },
                new Game { Name = "Speed Runner", Description = "Fast paced platformer" },
                new Game { Name = "Doom", Description = "Fast action shooter" },
                new Game { Name = "Subnautica", Description = "Underwater survival exploration" },
                new Game { Name = "Minecraft", Description = "Sandbox survival crafting" }
            );

            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetGame_WhenFound_ReturnsGame()
        {
            // Arrange
            var existing = DbContext.Games.First();
            var query = new GetGameQuery { Id = existing.Id };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Id.Should().Be(existing.Id);
            result.Name.Should().Be(existing.Name);
        }

        [Fact]
        public async Task GetGame_WhenNotFound_ThrowsError()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(new GetGameQuery { Id = 9999 }, CancellationToken.None));
        }


        [Fact]
        public async Task Search_NoFilters_ReturnsAll()
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(6);
        }

        [Theory]
        [InlineData("Need", "Need for Speed")]
        [InlineData("Gran", "Gran Turismo")]
        [InlineData("Spee", "Speed Runner")]
        [InlineData("Do", "Doom")]
        [InlineData("Sub", "Subnautica")]
        [InlineData("Mine", "Minecraft")]
        public async Task Search_NameStartsWith_ReturnRecord(string queryText, string excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                Name = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(excpectedResult);
        }

        [Theory]
        [InlineData("Arcade", "Need for Speed")]
        [InlineData("sim", "Gran Turismo")]
        [InlineData("fast p", "Speed Runner")]
        [InlineData("fast a", "Doom")]
        [InlineData("underwater", "Subnautica")]
        [InlineData("sand", "Minecraft")]
        public async Task Search_DescriptionStartsWith_ReturnRecord(string queryText, string excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                Description = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(excpectedResult);
        }

        [Fact]
        public async Task Search_DescriptionStartsWith_ReturnMultipleRecords()
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                Description = "Fast",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(2);
            result.Data.Select(x => x.Name)
                .Should().Contain(new[] { "Speed Runner", "Doom" });
        }

        [Theory]
        [InlineData("cade", "Need for Speed")]
        [InlineData("ulation", "Gran Turismo")]
        [InlineData("paced", "Speed Runner")]
        [InlineData("action", "Doom")]
        [InlineData("explor", "Subnautica")]
        [InlineData("craft", "Minecraft")]
        public async Task Search_SearchTermContains_ReturnsSingle(string queryText, string excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(excpectedResult);
        }

        [Theory]
        [InlineData("survival", new[] { "Subnautica", "Minecraft" })]
        [InlineData("racing", new[] { "Need for Speed", "Gran Turismo" })]
        [InlineData("fast", new[] { "Speed Runner", "Doom" })]
        [InlineData("ion", new[] { "Gran Turismo","Doom", "Subnautica" })]
        public async Task Search_SearchTermContains_ReturnsMultiple(string queryText,string[] excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(excpectedResult.Count());
            result.Data.Select(x => x.Name).Should().Contain(excpectedResult);
        }

        [Fact]
        public async Task Search_IsCaseInsensitive()
        {
            var query = new SearchGamesQuery
            {
                SearchTerms = "CRAFTING",
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be("Minecraft");
        }

    }
}
