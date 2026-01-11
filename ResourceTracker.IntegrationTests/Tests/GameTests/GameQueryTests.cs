

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.GameQueries;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;
using System.Reflection.Metadata;

namespace ResourceTracker.IntegrationTests.Tests.GameTests
{
    public class GameQueryTests : IntegrationTestBase
    {
        private GameQueriesHandler _handler;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _handler = new GameQueriesHandler(_serviceScope.ServiceProvider.GetRequiredService<IResourceTrackerRepository>());

            await SeedGames();
        }

        private async Task SeedGames()
        {
            _dbContext.Games.AddRange(
                new Game { Name = "Need for Speed", Description = "Arcade racing game" },
                new Game { Name = "Gran Turismo", Description = "Simulation racing" },
                new Game { Name = "Speed Runner", Description = "Fast paced platformer" },
                new Game { Name = "Doom", Description = "Fast action shooter" },
                new Game { Name = "Subnautica", Description = "Underwater survival exploration" },
                new Game { Name = "Minecraft", Description = "Sandbox survival crafting" }
            );

            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetGame_ReturnsGame_WhenFound()
        {
            // Arrange
            var existing = _dbContext.Games.First();
            var query = new GetGameQuery { Id = existing.Id };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Id.Should().Be(existing.Id);
            result.Name.Should().Be(existing.Name);
        }

        [Fact]
        public async Task GetGame_ThrowsError_WhenNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new GetGameQuery { Id = 9999 }, CancellationToken.None));
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
            var result = await _handler.Handle(query, CancellationToken.None);

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
        public async Task Search_Name_StartsWith_ReturnRecord(string queryText, string excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                Name = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

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
        public async Task Search_Description_StartsWith_ReturnRecord(string queryText, string excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                Description = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(excpectedResult);
        }

        [Fact]
        public async Task Search_Description_StartsWith_ReturnMultipleRecords()
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                Description = "Fast",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

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
        public async Task Search_SearchTerm_Contains_ReturnsSingle(string queryText, string excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(excpectedResult);
        }

        [Theory]
        [InlineData("survival", new[] { "Subnautica", "Minecraft" })]
        [InlineData("racing", new[] { "Need for Speed", "Gran Turismo" })]
        [InlineData("fast", new[] { "Speed Runner", "Doom" })]
        [InlineData("ion", new[] { "Gran Turismo","Doom", "Subnautica" })]
        public async Task Search_SearchTerm_Contains_ReturnsMultiple(string queryText,string[] excpectedResult)
        {
            // Arrange
            var query = new SearchGamesQuery
            {
                SearchTerms = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

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

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be("Minecraft");
        }

    }
}
