using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using ResourceTracker.Application.Services;
using ResourceTracker.Tests.Fixtures;
using ResourceTracker.Tests.TestData.ExpectedOutputs.Subnautica;

namespace ResourceTracker.Tests.Tests
{
    public class ImportServiceRecipeTests : IClassFixture<PrawnSuitFixture<GameDataDto>>
    {
        private readonly GameDataDto _gameData;

        public ImportServiceRecipeTests(PrawnSuitFixture<GameDataDto> fixture)
        {
            _gameData = fixture.Data;
        }
        [Fact]
        public void ImportRecipes_ShouldPass_ImportAllRecipes()
        {
            // Arrange
            var components = PrawnSuit.ExpectedComponentData();
            // Act
            var recipes = ImportService.ImportRecipes(_gameData, 1, components, default);
            // Assert
            Assert.Equal(PrawnSuit.ExpectedRecipeData().Count(), recipes.Count);
        }

        [Fact]
        public void ImportRecipes_ShouldPass_ImportCorrectRecipeData()
        {
            // Arrange
            var components = PrawnSuit.ExpectedComponentData();
            var expectedRecipes = PrawnSuit.ExpectedRecipeData();
            // Act
            var recipes = ImportService.ImportRecipes(_gameData, 1, components, default);
            // Assert
            // Group recipes by ComponentId to handle multiple recipes per component
            var groupedExpected = expectedRecipes.GroupBy(r => r.ComponentId);
            var groupedActual = recipes.GroupBy(r => r.ComponentId);

            foreach (var actualGroup in groupedActual)
            {
                var componentId = actualGroup.Key;
                var expectedGroup = groupedExpected.SingleOrDefault(g => g.Key == componentId);
                Assert.NotNull(expectedGroup); // ensure we expected this component

                Assert.Equal(expectedGroup.Count(), actualGroup.Count());

                foreach (var actual in actualGroup)
                {
                    var match = expectedGroup.SingleOrDefault(e =>
                        e.AmountMade == actual.AmountMade &&
                        e.RequiredFacilityId == actual.RequiredFacilityId);

                    Assert.NotNull(match); // ensure a matching recipe exists
                }
            }
        }
    }
}
