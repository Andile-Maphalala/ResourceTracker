using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.ComponentTests
{
    public class ComponentQueryTests : IntegrationTestBase
    {
        public ComponentQueryTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            DbContext.Games.AddRange(
                new Game { Name = "Terraria", Description = "2D sandbox" },
                new Game { Name = "Factorio", Description = "Automation and factory building" },
                new Game { Name = "Stardew Valley", Description = "Farming RPG" }
            );

            await DbContext.SaveChangesAsync();

            Game terraria = DbContext.Games.First(g => g.Name == "Terraria");
            Game factorio = DbContext.Games.First(g => g.Name == "Factorio");
            Game stardew = DbContext.Games.First(g => g.Name == "Stardew Valley");

            DbContext.Components.AddRange(
                new Component { Name = "Copper Ore", Description = "Basic ore", GameId = terraria.Id, Type = 1 },
                new Component { Name = "Iron Ore", Description = "Smelt into bars", GameId = terraria.Id, Type = 1 },
                new Component { Name = "Conveyor Belt", Description = "Transports items", GameId = factorio.Id, Type = 2 },
                new Component { Name = "Assembler", Description = "Automates crafting", GameId = factorio.Id, Type = 2 },
                new Component { Name = "Parsnip Seed", Description = "Farming seed", GameId = stardew.Id, Type = 3 }
            );

            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetComponent_WhenFound_ReturnsComponent()
        {
            Component existing = DbContext.Components.Include(c => c.Game).First(c => c.Name == "Iron Ore");
            GetComponentQuery query = new GetComponentQuery(existing.Id);

            GetComponentResponse result = await Sender.Send(query, CancellationToken.None);

            result.Id.Should().Be(existing.Id);
            result.Name.Should().Be(existing.Name);
            result.Description.Should().Be(existing.Description);
            result.Type.Should().Be(existing.Type);
            result.GameId.Should().Be(existing.GameId);
            result.GameName.Should().Be(existing.Game.Name);
            result.PictureId.Should().Be(existing.PictureId);
        }

        [Fact]
        public async Task GetComponent_WhenNotFound_ThrowsNotFoundException()
        {
            GetComponentQuery query = new GetComponentQuery(999999);

            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (Component) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task Search_NoFilters_ReturnsAll()
        {
            SearchComponentsQuery query = new SearchComponentsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Copper", "Copper Ore")]
        [InlineData("Conveyor", "Conveyor Belt")]
        [InlineData("Parsnip", "Parsnip Seed")]
        public async Task Search_NameStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            SearchComponentsQuery query = new SearchComponentsQuery
            {
                Name = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("smelt", "Iron Ore")]
        [InlineData("transports", "Conveyor Belt")]
        [InlineData("farming", "Parsnip Seed")]
        public async Task Search_DescriptionStartsWith_ReturnRecord(string queryText, string expectedName)
        {
            SearchComponentsQuery query = new SearchComponentsQuery
            {
                Description = queryText,
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be(expectedName);
        }

        [Fact]
        public async Task Search_WithGameId_ReturnsOnlyComponentsForThatGame()
        {
            var terraria = DbContext.Games.First(g => g.Name == "Terraria");
            var query = new SearchComponentsQuery
            {
                GameId = terraria.Id,
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().HaveCount(2);
            result.Data.Select(c => c.Name).Should().Contain(new[] { "Copper Ore", "Iron Ore" });
        }

        [Fact]
        public async Task Search_WithGameIdAndNameFilter_ReturnsSingleRecord()
        {
            var factorio = DbContext.Games.First(g => g.Name == "Factorio");
            var query = new SearchComponentsQuery
            {
                GameId = factorio.Id,
                Name = "Conveyor",
                PageNumber = 1,
                PageSize = 10
            };

            PageableResponse<SearchComponentsResponse> result = await Sender.Send(query, CancellationToken.None);

            result.Data.Should().ContainSingle();
            result.Data.First().Name.Should().Be("Conveyor Belt");
        }
    }
}