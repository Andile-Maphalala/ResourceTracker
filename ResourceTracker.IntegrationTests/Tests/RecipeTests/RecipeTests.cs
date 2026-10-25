using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.RecipeCommands.CreateRecipe;
using ResourceTracker.Application.Features.Commands.RecipeCommands.DeleteRecipe;
using ResourceTracker.Application.Features.Commands.RecipeCommands.UpdateRecipe;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipe;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipesForComponent;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.RecipeTests
{
    public class RecipeTests : IntegrationTestBase
    {
        private int _gameId;
        private int _widgetId;
        private int _ironId;
        private int _woodId;
        private int _forgeId;
        private int _gearId;

        public RecipeTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            SetupAdminUser();

            _gameId = await AddGame("Recipe Test Game");
            _widgetId = await AddComponent("Widget", ComponentTypeEnum.Composite);
            _ironId = await AddComponent("Iron", ComponentTypeEnum.Resource);
            _woodId = await AddComponent("Wood", ComponentTypeEnum.Resource);
            _forgeId = await AddComponent("Forge", ComponentTypeEnum.Facility);
            _gearId = await AddComponent("Gear", ComponentTypeEnum.Composite);
        }

        [Fact]
        public async Task CreateRecipe_PersistsRecipeAndIngredients()
        {
            var created = await Create(_widgetId, amountMade: 2, _forgeId, level: 3, (_ironId, 4), (_woodId, 5));

            var recipe = await Sender.Send(new GetRecipeQuery(created.Id), CancellationToken.None);

            recipe.ComponentId.Should().Be(_widgetId);
            recipe.ComponentName.Should().Be("Widget");
            recipe.AmountMade.Should().Be(2);
            recipe.RequiredFacilityId.Should().Be(_forgeId);
            recipe.RequiredFacilityName.Should().Be("Forge");
            recipe.RequiredFacilityLevel.Should().Be(3);
            recipe.Ingredients.Should().HaveCount(2);
            recipe.Ingredients.Should().Contain(i => i.ComponentId == _ironId && i.AmountRequired == 4);
            recipe.Ingredients.Should().Contain(i => i.ComponentId == _woodId && i.AmountRequired == 5);
        }

        [Fact]
        public async Task CreateRecipe_WithoutStation_ForcesLevelToDefault()
        {
            var created = await Create(_widgetId, amountMade: 1, facilityId: null, level: 5, (_ironId, 1));

            var recipe = await Sender.Send(new GetRecipeQuery(created.Id), CancellationToken.None);

            recipe.RequiredFacilityId.Should().BeNull();
            recipe.RequiredFacilityLevel.Should().Be(1);
        }

        [Fact]
        public async Task CreateRecipe_NonAdmin_Throws()
        {
            SetupNonAdminUser();

            await Assert.ThrowsAsync<BadRequestException>(() =>
                Create(_widgetId, 1, null, 1, (_ironId, 1)));
        }

        [Fact]
        public async Task CreateRecipe_UnknownOutputComponent_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                Create(IncorrectValue, 1, null, 1, (_ironId, 1)));
        }

        [Fact]
        public async Task CreateRecipe_IngredientIsTheOutput_Throws()
        {
            // Self-reference would make the requirement resolver recurse forever.
            var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
                Create(_widgetId, 1, null, 1, (_widgetId, 1)));

            ex.Message.Should().Contain("own output");
        }

        [Fact]
        public async Task CreateRecipe_DuplicateIngredient_Throws()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Create(_widgetId, 1, null, 1, (_ironId, 1), (_ironId, 2)));
        }

        [Fact]
        public async Task CreateRecipe_StationIsNotAFacility_Throws()
        {
            var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
                Create(_widgetId, 1, _gearId, 1, (_ironId, 1)));

            ex.Message.Should().Contain("not a facility");
        }

        [Fact]
        public async Task CreateRecipe_SecondRecipeForSameOutput_Throws()
        {
            // Both resolvers take the first recipe per output, so a second would be ignored.
            await Create(_widgetId, 1, null, 1, (_ironId, 1));

            var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
                Create(_widgetId, 1, null, 1, (_woodId, 1)));

            ex.Message.Should().Contain("already exists");
        }

        [Fact]
        public async Task UpdateRecipe_ReplacesIngredientsWholesale()
        {
            var created = await Create(_widgetId, amountMade: 1, facilityId: null, level: 1, (_ironId, 1), (_woodId, 1));

            await Sender.Send(new UpdateRecipeCommand
            {
                Id = created.Id,
                ComponentId = _widgetId,
                AmountMade = 3,
                RequiredFacilityId = _forgeId,
                RequiredFacilityLevel = 2,
                Ingredients = new List<RecipeIngredientDto>
                {
                    new RecipeIngredientDto { ComponentId = _woodId, AmountRequired = 7 }
                }
            }, CancellationToken.None);

            var recipe = await Sender.Send(new GetRecipeQuery(created.Id), CancellationToken.None);

            recipe.AmountMade.Should().Be(3);
            recipe.RequiredFacilityId.Should().Be(_forgeId);
            recipe.RequiredFacilityLevel.Should().Be(2);
            recipe.Ingredients.Should().ContainSingle();
            recipe.Ingredients.Single().ComponentId.Should().Be(_woodId);
            recipe.Ingredients.Single().AmountRequired.Should().Be(7);
        }

        [Fact]
        public async Task DeleteRecipe_RemovesIngredientsWithIt()
        {
            var created = await Create(_widgetId, 1, null, 1, (_ironId, 1), (_woodId, 1));

            await Sender.Send(new DeleteRecipeCommand(created.Id), CancellationToken.None);

            (await DbContext.Recipes.CountAsync(r => r.Id == created.Id)).Should().Be(0);
            (await DbContext.Set<RecipeComponent>().CountAsync(rc => rc.RecipeId == created.Id)).Should().Be(0);
        }

        [Fact]
        public async Task GetRecipesForComponent_ReturnsTheRecipeWithIngredients()
        {
            var created = await Create(_widgetId, 1, null, 1, (_ironId, 2));

            var recipes = await Sender.Send(new GetRecipesForComponentQuery(_widgetId), CancellationToken.None);

            recipes.Should().ContainSingle();
            recipes.Single().Id.Should().Be(created.Id);
            recipes.Single().ComponentName.Should().Be("Widget");
            recipes.Single().Ingredients.Should().ContainSingle(i => i.ComponentId == _ironId && i.AmountRequired == 2);
        }

        [Fact]
        public async Task GetRecipesForComponent_ComponentWithNoRecipe_ReturnsEmpty()
        {
            var recipes = await Sender.Send(new GetRecipesForComponentQuery(_woodId), CancellationToken.None);

            recipes.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRecipesForComponent_UnknownComponent_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(new GetRecipesForComponentQuery(IncorrectValue), CancellationToken.None));
        }

        [Fact]
        public async Task GetRecipe_UnknownId_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(new GetRecipeQuery(IncorrectValue), CancellationToken.None));
        }

        #region helpers

        private Task<CreateRecipeResponse> Create(
            int outputComponentId,
            int amountMade,
            int? facilityId,
            int level,
            params (int componentId, int amount)[] ingredients)
        {
            return Sender.Send(new CreateRecipeCommand
            {
                ComponentId = outputComponentId,
                AmountMade = amountMade,
                RequiredFacilityId = facilityId,
                RequiredFacilityLevel = level,
                Ingredients = ingredients
                    .Select(i => new RecipeIngredientDto { ComponentId = i.componentId, AmountRequired = i.amount })
                    .ToList()
            }, CancellationToken.None);
        }

        private async Task<int> AddGame(string name)
        {
            var game = new Game { Name = name, Description = "Recipe tests" };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

        private async Task<int> AddComponent(string name, ComponentTypeEnum type)
        {
            var component = new Component
            {
                Name = name,
                Description = "Recipe tests",
                GameId = _gameId,
                Type = (int)type
            };
            DbContext.Components.Add(component);
            await DbContext.SaveChangesAsync();
            return component.Id;
        }

        #endregion
    }
}
