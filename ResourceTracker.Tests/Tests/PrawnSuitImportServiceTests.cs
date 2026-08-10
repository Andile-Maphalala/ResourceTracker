using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using ResourceTracker.Application.Services;
using ResourceTracker.Domain.Enums;
using ResourceTracker.Tests.TestData.Fixtures;

namespace ResourceTracker.Tests.Tests
{
    public class PrawnSuitImportServiceTests : IClassFixture<PrawnSuitFixture<GameDataDto>>
    {
        private readonly GameDataDto _gameData;

        public PrawnSuitImportServiceTests(PrawnSuitFixture<GameDataDto> fixture)
        {
            _gameData = fixture.Data;
        }



        [Fact]

        public void Import_SubnauticaComponents_ShouldImportAllComponents()
        {
            // Arrange
            // Act
            var components = ImportService.ImportComponents(_gameData, 1);
            // Assert
            Assert.Equal(_gameData.Components.Count, components.Count);
            for (int i = 0; i < components.Count; i++)
            {
                Assert.Equal(_gameData.Components[i].Name, components[i].Name);
                Assert.Equal(_gameData.Components[i].Description, components[i].Description);
            }
        }

        [Fact]
        public void Import_SubnauticaComponents_ShouldHave_CorrectNumber_Resource()
        {
            // Arrange
            // Act
            var components = ImportService.ImportComponents(_gameData, 1);
            // Assert
            var matchingRecords = components.Where(x => x.Type == (int)ComponentTypeEnum.Resource).ToList();
            Assert.Equal(14, matchingRecords.Count());
        }

        [Fact]
        public void Import_SubnauticaComponents_ShouldHave_CorrectNumber_Composite()
        {
            // Arrange
            // Act
            var components = ImportService.ImportComponents(_gameData, 1);
            // Assert
            var matchingRecords = components.Where(x => x.Type == (int)ComponentTypeEnum.Composite).ToList();
            Assert.Equal(14, matchingRecords.Count());
        }

        [Fact]
        public void Import_SubnauticaComponents_ShouldHave_CorrectNumber_Facility ()
        {
            // Arrange
            // Act
            var components = ImportService.ImportComponents(_gameData, 1);
            // Assert
            var matchingRecords = components.Where(x => x.Type == (int)ComponentTypeEnum.Facility).ToList();
            Assert.Equal(3, matchingRecords.Count());
        }

    }
}
