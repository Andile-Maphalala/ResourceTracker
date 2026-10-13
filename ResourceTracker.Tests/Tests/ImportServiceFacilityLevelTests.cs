using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using ResourceTracker.Application.Services;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Tests.Tests
{
    public class ImportServiceFacilityLevelTests
    {
        private const int ForgeId = 10;
        private const int PlateId = 20;
        private const int IronId = 30;

        private static List<Component> Components() => new()
        {
            new Component { Id = ForgeId, Name = "Forge", Type = (int)ComponentTypeEnum.Facility },
            new Component { Id = PlateId, Name = "Plate", Type = (int)ComponentTypeEnum.Composite },
            new Component { Id = IronId, Name = "Iron", Type = (int)ComponentTypeEnum.Resource }
        };

        /// <summary>
        /// One composite, "Plate", crafted at an optional station with an optional level. A null
        /// stationLevel leaves StationLevel at its initialised default, which is what the
        /// deserializer does when the file omits the field entirely.
        /// </summary>
        private static GameDataDto GameData(string? station, int? stationLevel)
        {
            var recipe = new RecipeDto
            {
                OutputQuantity = 1,
                Station = station,
                Requirements = new List<ComponentRequirementDto>
                {
                    new ComponentRequirementDto { Compononent = "Iron", Quantity = 2 }
                }
            };

            if (stationLevel.HasValue)
            {
                recipe.StationLevel = stationLevel.Value;
            }

            return new GameDataDto
            {
                Name = "Test",
                Components = new List<GameComponentDto>
                {
                    new GameComponentDto { Name = "Forge", Type = "Facility" },
                    new GameComponentDto { Name = "Plate", Type = "Composite", Recipe = recipe }
                }
            };
        }

        [Fact]
        public void ImportRecipes_Maps_StationLevel_Onto_RequiredFacilityLevel()
        {
            var recipes = ImportService.ImportRecipes(GameData("Forge", 4), 1, Components(), default);

            var recipe = Assert.Single(recipes);
            Assert.Equal(ForgeId, recipe.RequiredFacilityId);
            Assert.Equal(4, recipe.RequiredFacilityLevel);
        }

        [Fact]
        public void ImportRecipes_Defaults_FacilityLevel_To_1_When_StationLevel_Is_Absent()
        {
            // Subnautica's files have no station_level at all, so every one of its recipes lands here.
            var recipes = ImportService.ImportRecipes(GameData("Forge", null), 1, Components(), default);

            var recipe = Assert.Single(recipes);
            Assert.Equal(ForgeId, recipe.RequiredFacilityId);
            Assert.Equal(1, recipe.RequiredFacilityLevel);
        }

        [Fact]
        public void ImportRecipes_Treats_StationLevel_0_As_Level_1()
        {
            // Valheim writes 0 for "any level of this station", which is not a requirement of zero.
            var recipes = ImportService.ImportRecipes(GameData("Forge", 0), 1, Components(), default);

            var recipe = Assert.Single(recipes);
            Assert.Equal(ForgeId, recipe.RequiredFacilityId);
            Assert.Equal(1, recipe.RequiredFacilityLevel);
        }

        [Fact]
        public void ImportRecipes_Allows_A_Recipe_With_No_Station()
        {
            // Hand-crafted, like Valheim's Hammer and Torch: no station is not an error.
            var recipes = ImportService.ImportRecipes(GameData(null, null), 1, Components(), default);

            var recipe = Assert.Single(recipes);
            Assert.Null(recipe.RequiredFacilityId);
            Assert.Equal(1, recipe.RequiredFacilityLevel);
        }

        [Fact]
        public void ImportRecipes_Throws_When_The_Station_Is_Not_Declared()
        {
            // The failure this guards against: an undeclared station used to import as a null
            // facility, so the requirement disappeared with nothing left to notice it had.
            var ex = Assert.Throws<BadRequestException>(() =>
                ImportService.ImportRecipes(GameData("Cyclops Upgrade Console", 1), 1, Components(), default));

            Assert.Contains("Cyclops Upgrade Console", ex.Message);
        }

        [Fact]
        public void ImportRecipes_Throws_When_The_Recipe_Component_Is_Not_Declared()
        {
            // The recipe names an output that was never imported as a component.
            var components = Components().Where(c => c.Name != "Plate").ToList();

            var ex = Assert.Throws<BadRequestException>(() =>
                ImportService.ImportRecipes(GameData("Forge", 1), 1, components, default));

            Assert.Contains("Plate", ex.Message);
        }
    }
}
