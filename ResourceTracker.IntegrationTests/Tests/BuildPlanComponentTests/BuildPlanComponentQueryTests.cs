using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.GetBuildPlanComponent;
using ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.SearchBuildPlanComponent;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanComponentTests
{
    public class BuildPlanComponentQueryTests : IntegrationTestBase
    {
        public BuildPlanComponentQueryTests(IntegrationTestFixture fixture) : base(fixture)
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

            // Seed build plans
            var coral = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base");
            var deep = DbContext.GameSaves.First(gs => gs.Name == "Deep Sea Exploration");
            DbContext.BuildPlans.AddRange(
                new BuildPlan { Name = "Base Assembly", Description = "Starter build plan", GameSaveId = coral.Id },
                new BuildPlan { Name = "Deep Rig", Description = "Advanced build plan", GameSaveId = coral.Id },
                new BuildPlan { Name = "Surface Station", Description = "Surface construction", GameSaveId = deep.Id }
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

            // Seed build plan components (links)
            var bpBase = DbContext.BuildPlans.First(bp => bp.Name == "Base Assembly");
            var bpDeep = DbContext.BuildPlans.First(bp => bp.Name == "Deep Rig");
            var copper = DbContext.Components.First(c => c.Name == "Copper Ore");
            var iron = DbContext.Components.First(c => c.Name == "Iron Ingot");
            var conveyor = DbContext.Components.First(c => c.Name == "Conveyor Belt");

            DbContext.BuildPlanComponents.AddRange(
                new BuildPlanComponent { BuildPlanId = bpBase.Id, ComponentId = copper.Id, Order = 1, QuantityNeeded = 3 },
                new BuildPlanComponent { BuildPlanId = bpBase.Id, ComponentId = iron.Id, Order = 2, QuantityNeeded = 1 },
                new BuildPlanComponent { BuildPlanId = bpDeep.Id, ComponentId = conveyor.Id, Order = 1, QuantityNeeded = 5 }
            );
            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetBuildPlanComponent_WhenFound_ReturnsBuildPlanComponent()
        {
            // Arrange
            var existing = DbContext.BuildPlanComponents.Include(x => x.Component).Include(x => x.BuildPlan).First();
            var query = new GetBuildPlanComponentQuery(existing.Id);

            // Act
            var result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Id.Should().Be(existing.Id);
            result.Order.Should().Be(existing.Order);
            result.QuantityNeeded.Should().Be(existing.QuantityNeeded);
            result.ComponentName.Should().Be(existing.Component.Name);
            result.ComponentType.Should().Be(existing.Component.Type);
            result.BuildPlanId.Should().Be(existing.BuildPlanId);
            result.BuildPlanName.Should().Be(existing.BuildPlan.Name);
        }

        [Fact]
        public async Task GetBuildPlanComponent_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var query = new GetBuildPlanComponentQuery(99999);

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(query, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlanComponent) with Key ({query.Id}) was not found.");
        }

        [Fact]
        public async Task Search_NoFilters_ReturnsAll()
        {
            // Arrange
            var query = new SearchBuildPlanComponentQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchBuildPlanComponentResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(DbContext.BuildPlanComponents.Count());
        }

        [Fact]
        public async Task Search_WithComponentId_ReturnsSingleRecord()
        {
            // Arrange
            var component = DbContext.Components.First(c => c.Name == "Copper Ore");
            var query = new SearchBuildPlanComponentQuery
            {
                ComponentId = component.Id,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchBuildPlanComponentResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be("Copper Ore");
        }

        [Fact]
        public async Task Search_WithBuildPlanId_ReturnsOnlyComponentsForThatBuildPlan()
        {
            // Arrange
            var bp = DbContext.BuildPlans.First(bp => bp.Name == "Base Assembly");
            var query = new SearchBuildPlanComponentQuery
            {
                BuildPlanId = bp.Id,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchBuildPlanComponentResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(2);
            result.Data.Select(x => x.ComponentName).Should().Contain(new[] { "Copper Ore", "Iron Ingot" });
        }

        [Fact]
        public async Task Search_IsCaseInsensitive()
        {
            // Arrange
            var query = new SearchBuildPlanComponentQuery
            {
                SearchTerms = "COPPER",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            PageableResponse<SearchBuildPlanComponentResponse> result = await Sender.Send(query, CancellationToken.None);

            // Assert
            result.Data.Should().ContainSingle();
            result.Data.First().ComponentName.Should().Be("Copper Ore");
        }
    }
}
