using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using ResourceTracker.Tests.Fixtures;


namespace ResourceTracker.Tests.Tests
{
    public class GameDataDtoTests : IClassFixture<PrawnSuitFixture<GameDataDto>>
    {
        private readonly GameDataDto _gameData;
        public GameDataDtoTests(PrawnSuitFixture<GameDataDto> fixture)
        {
            _gameData = fixture.Data;
        }

        [Fact]
        public void Should_Load_GameData_From_File()
        {
            Assert.NotNull(_gameData);
            Assert.NotEmpty(_gameData.Components);
            Assert.All(_gameData.Components, c => Assert.NotNull(c.Name));
            Assert.All(_gameData.Components.Where(c => c.Recipe != null), c =>
            {
                Assert.NotEmpty(c.Recipe!.Requirements);
            });
        }

        [Fact]
        public void Should_Have_Exact_Number_Of_Components()
        {
            Assert.Equal(31, _gameData.Components.Count);
        }

        [Fact]
        public void Should_Have_Exact_Number_Of_Recipes()
        {
            var componentsWithRecipes = _gameData.Components.Where(c => c.Recipe != null).ToList();
            Assert.Equal(17, componentsWithRecipes.Count);
        }


        [Fact]
        public void Should_Have_Exact_Number_Of_requirements()
        {
            var requierments = _gameData.Components
                .Where(c => c.Recipe != null)
                .SelectMany(c => c.Recipe!.Requirements)
                .ToList();
            Assert.Equal(34, requierments.Count);
        }
    }
}
