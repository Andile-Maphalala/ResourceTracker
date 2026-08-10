using FluentAssertions;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;
using ResourceTracker.Tests.TestData.TestData.ExpectedOutputs.Subnautica;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanRequirementTests
{
    public class BuildPlanRequirementQueryPrawnSuitTests : IntegrationTestBase
    {
        private int _GameSaveId;
        private int _buildPlanId;
        private int _questId;
        public BuildPlanRequirementQueryPrawnSuitTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            SetupNonAdminUser();

            //Seed game data
            DbContext.Games.AddRange(
                 new Game { Name = "Need for Speed", Description = "Arcade racing game" },
                 new Game { Name = "Gran Turismo", Description = "Simulation racing" },
                 new Game { Name = "Speed Runner", Description = "Fast paced platformer" },
                 new Game { Name = "Doom", Description = "Fast action shooter" },
                 new Game { Name = "Subnautica", Description = "Underwater survival exploration" },
                 new Game { Name = "Minecraft", Description = "Sandbox survival crafting" }
             );
            await DbContext.SaveChangesAsync();

            var _GameId = DbContext.Games.First(g => g.Name == "Subnautica").Id;
            DbContext.GameSaves.AddRange(
                new GameSave { GameId = _GameId, Name = "Coral Reef Base", Description = "Save1", UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = _GameId, Name = "Deep Sea Exploration", Description = "Save2", UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = _GameId, Name = "Playing Around", Description = "Save3", UserId = UserInfoMock.Object.GetUserId() },
                new GameSave { GameId = _GameId, Name = "First", Description = "Save1", UserId = AdminUserId }
            );
            await DbContext.SaveChangesAsync();
            _GameSaveId = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base").Id;

            var prawnSuitComponets = PrawnSuitComponentOutput.ExpectedComponentData();
            foreach (var component in prawnSuitComponets)
            {
                component.GameId = _GameId;
            }
            DbContext.Components.AddRange(prawnSuitComponets);
            await DbContext.SaveChangesAsync();

            var prawnSuitRecipes = PrawnSuitComponentOutput.ExpectedRecipeData();
            DbContext.Recipes.AddRange(prawnSuitRecipes);
            await DbContext.SaveChangesAsync();
            var prawnSuitId = DbContext.Components.First(c => c.Name == "Prawn Suit").Id;

            //Seed Quest Data
            var quest = new Quest
            {
                GameSaveId = _GameSaveId,
                Name = "Find the Prawn Suit",
                Description = "Locate and acquire the Prawn Suit for deep-sea exploration.",
                Location = "Jellyshroom Cave"
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            _questId = quest.Id;


            var buildPlan = new BuildPlan
            {
                GameSave = DbContext.GameSaves.First(gs => gs.Name == "Coral Reef Base"),
                Name = "Prawn Suit Only",
                Description = "Build plan focusing on Prawn Suit components"
            };

            DbContext.BuildPlans.Add(buildPlan);
            await DbContext.SaveChangesAsync();
            _buildPlanId = buildPlan.Id;

            var buildplanQuest = new BuildPlanQuest
            {
                BuildPlanId = buildPlan.Id,
                QuestId = quest.Id
            };
            DbContext.BuildPlanQuests.Add(buildplanQuest);
            await DbContext.SaveChangesAsync();

            var buildPlanComponents = new BuildPlanComponent
            {
                Order = 1,
                QuantityNeeded = 1,
                ComponentId = prawnSuitId,
                BuildPlanId = buildPlan.Id
            };
            DbContext.BuildPlanComponents.Add(buildPlanComponents);
            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetBuildPlanRequirement_EmptyInventoryOnlyComponets_ShouldReturnCorrectRequirements()
        {
            // Arrange
            var query = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = _buildPlanId,
                IncludeFacilityRequirements = false,
                IncludeInventory = false
            };
            // Act
            var result = await Sender.Send(query);

            // Assert
            var expectedRequirements = PrawnSuitBuildPlanOutput.ExpectedResponseDataNoFacilityNoInvestory();

            result.Should().NotBeNull();
            result.BuildPlanId.Should().Be(_buildPlanId);
            result.BuildPlanName.Should().Be("Prawn Suit Only");
            result.Requirements.Should().HaveCount(8);
            result.TotalAvailable.Should().Be(expectedRequirements.Sum(r => r.AvailableAmount));
            result.TotalMissing.Should().Be(expectedRequirements.Sum(r => r.MissingAmount));
            result.TotalRequired.Should().Be(expectedRequirements.Sum(r => r.RequiredAmount));
            foreach(var expected in expectedRequirements)
            {
                var requirement = result.Requirements.FirstOrDefault(r => r.ComponentName == expected.ComponentName);

                requirement.Should().NotBeNull();

                requirement.ComponentId.Should().Be(expected.ComponentId);
                requirement.RequiredAmount.Should().Be(expected.RequiredAmount);
                requirement.AvailableAmount.Should().Be(expected.AvailableAmount);
                requirement.MissingAmount.Should().Be(expected.MissingAmount);
            }
        }

        [Fact]
        public async Task GetBuildPlanRequirement_WithResourcesInInventory_ShouldReturnCorrectRequirements()
        {
            // Arrange
            var query = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = _buildPlanId,
                IncludeFacilityRequirements = false,
                IncludeInventory = true
            };
            //so should return 1 diamond nedded, 0 gel sacks needed, and 7 titanium needed, rest the same
            await ClearInventorySeed();
            await SeedInventoryAsync(
            (30, 1),// Diamond 2-1
            (24,2),  // Gell Sack 2-2
            (14,13) // Titanium 20-13
            );


            // Act
            var result = await Sender.Send(query);

            // Assert
            var expectedRequirements = PrawnSuitBuildPlanOutput.ExpectedResponseDataNoFacilityWithInvestoryOnlyResources();

            result.Should().NotBeNull();
            result.BuildPlanId.Should().Be(_buildPlanId);
            result.BuildPlanName.Should().Be("Prawn Suit Only");
            result.Requirements.Should().HaveCount(8);
            result.TotalAvailable.Should().Be(expectedRequirements.Sum(r => r.AvailableAmount));
            result.TotalMissing.Should().Be(expectedRequirements.Sum(r => r.MissingAmount));
            result.TotalRequired.Should().Be(expectedRequirements.Sum(r => r.RequiredAmount));
            foreach (var expected in expectedRequirements)
            {
                var requirement = result.Requirements.FirstOrDefault(r => r.ComponentName == expected.ComponentName);

                requirement.Should().NotBeNull();

                requirement.ComponentId.Should().Be(expected.ComponentId);
                requirement.RequiredAmount.Should().Be(expected.RequiredAmount);
                requirement.AvailableAmount.Should().Be(expected.AvailableAmount);
                requirement.MissingAmount.Should().Be(expected.MissingAmount);
            }
        }

        [Fact]
        public async Task GetBuildPlanRequirement_WithResourcesAndCompositeInInventoryEnameldGlass_ShouldReturnCorrectRequirements()
        {
            // Arrange
            var query = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = _buildPlanId,
                IncludeFacilityRequirements = false,
                IncludeInventory = true
            };
            //so should return 1 diamond nedded, 0 gel sacks needed, and 7 titanium needed, rest the same
            await ClearInventorySeed();
            await SeedInventoryAsync(
            (30, 1),// Diamond 2-1
            (24,2),  // Gell Sack 2-2
            (14,13) // Titanium 20-13
            );

            //Should return 1 enameled glass needed
            //Not return : Quartz,Stalker tooth
            //rest same
            await SeedInventoryAsync(
            (26, 2));// Enameled Glass 2-1

            // Act
            var result = await Sender.Send(query);

            // Assert
            var expectedRequirements = PrawnSuitBuildPlanOutput.ExpectedResponseDataNoFacilityWithInvestoryCompositeAndResources_EnameldGlass();

            result.Should().NotBeNull();
            result.BuildPlanId.Should().Be(_buildPlanId);
            result.BuildPlanName.Should().Be("Prawn Suit Only");
            result.Requirements.Should().HaveCount(7);
            result.TotalAvailable.Should().Be(expectedRequirements.Sum(r => r.AvailableAmount));
            result.TotalMissing.Should().Be(expectedRequirements.Sum(r => r.MissingAmount));
            result.TotalRequired.Should().Be(expectedRequirements.Sum(r => r.RequiredAmount));
            foreach (var expected in expectedRequirements)
            {
                var requirement = result.Requirements.FirstOrDefault(r => r.ComponentName == expected.ComponentName);

                requirement.Should().NotBeNull();

                requirement.RequiredAmount.Should().Be(expected.RequiredAmount);
                requirement.AvailableAmount.Should().Be(expected.AvailableAmount);
                requirement.MissingAmount.Should().Be(expected.MissingAmount);
            }
        }

        [Fact]
        public async Task GetBuildPlanRequirement_WithResourcesAndCompositeInInventoryGlass_ShouldReturnCorrectRequirements()
        {
            // Arrange
            var query = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = _buildPlanId,
                IncludeFacilityRequirements = false,
                IncludeInventory = true
            };
            //so should return 1 diamond nedded, 0 gel sacks needed, and 7 titanium needed, rest the same
            await ClearInventorySeed();
            await SeedInventoryAsync(
            (30, 1),// Diamond 2-1
            (24, 2),  // Gell Sack 2-2
            (14, 13) // Titanium 20-13
            );

            //Should return 1 enameled glass needed
            //Not return : Quartz,Stalker tooth
            //rest same
            await SeedInventoryAsync(
            (28, 3));//Glass

            // Act
            var result = await Sender.Send(query);

            // Assert
            var expectedRequirements = PrawnSuitBuildPlanOutput.ExpectedResponseDataNoFacilityWithInvestoryCompositeAndResources_Glass();

            result.Should().NotBeNull();
            result.BuildPlanId.Should().Be(_buildPlanId);
            result.BuildPlanName.Should().Be("Prawn Suit Only");
            result.Requirements.Should().HaveCount(8);
            result.TotalAvailable.Should().Be(expectedRequirements.Sum(r => r.AvailableAmount));
            result.TotalMissing.Should().Be(expectedRequirements.Sum(r => r.MissingAmount));
            result.TotalRequired.Should().Be(expectedRequirements.Sum(r => r.RequiredAmount));
            foreach (var expected in expectedRequirements)
            {
                var requirement = result.Requirements.FirstOrDefault(r => r.ComponentName == expected.ComponentName);

                requirement.Should().NotBeNull();

                requirement.ComponentId.Should().Be(expected.ComponentId);
                requirement.RequiredAmount.Should().Be(expected.RequiredAmount);
                requirement.AvailableAmount.Should().Be(expected.AvailableAmount);
                requirement.MissingAmount.Should().Be(expected.MissingAmount);
            }
        }

        [Fact]
        public async Task GetBuildPlanRequirement_Should_DepreciateInventoryAcrossMultipleRecipeLevels()
        {
            // Arrange
            var query = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = _buildPlanId,
                IncludeFacilityRequirements = false,
                IncludeInventory = true
            };

            await ClearInventorySeed();

            // Seed partial composites + base resource
            await SeedInventoryAsync(
                (/* Titanium */ 14, 10),
                (/* Titanium Ingot */ 16, 1)
            );

            // Act
            var result = await Sender.Send(query);

            // Assert
            var titanium = result.Requirements.First(r => r.ComponentName == "Titanium");
            var titaniumIngot = result.Requirements.First(r => r.ComponentName == "Titanium Ingot");

            titanium.RequiredAmount.Should().Be(10);
            titanium.AvailableAmount.Should().Be(10);
            titanium.MissingAmount.Should().Be(0);

            titaniumIngot.RequiredAmount.Should().Be(2);
            titaniumIngot.AvailableAmount.Should().Be(1);
            titaniumIngot.MissingAmount.Should().Be(1);
        }

        [Fact]
        public async Task GetBuildPlanRequirement_WithFacility_WithResourcesAndCompositeInInventoryGlass_ShouldReturnCorrectRequirements()
        {
            //same as prevbious test but with faciluity requirments 
            // Arrange
            var query = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = _buildPlanId,
                IncludeFacilityRequirements = true,//incluse facility requirements
                IncludeInventory = true
            };
            //so should return 1 diamond nedded, 0 gel sacks needed, and 7 titanium needed, rest the same
            await ClearInventorySeed();
            await SeedInventoryAsync(
            (30, 1),// Diamond 2-1
            (24, 2),  // Gell Sack 2-2
            (14, 13) // Titanium 20-13
            );

            //Should return 1 enameled glass needed
            //Not return : Quartz,Stalker tooth
            //rest same
            await SeedInventoryAsync(
            (28, 3));//Glass

            // Act
            var result = await Sender.Send(query);

            // Assert
            var expectedRequirements = PrawnSuitBuildPlanOutput.ExpectedResponseDataWithFacilityyWithInvestoryCompositeAndResources_Glass();

            result.Should().NotBeNull();
            result.BuildPlanId.Should().Be(_buildPlanId);
            result.BuildPlanName.Should().Be("Prawn Suit Only");
            result.Requirements.Should().HaveCount(14);
            foreach (var expected in expectedRequirements)
            {
                var requirement = result.Requirements.FirstOrDefault(r => r.ComponentName == expected.ComponentName);

                requirement.Should().NotBeNull();

                requirement.ComponentId.Should().Be(expected.ComponentId);
                requirement.RequiredAmount.Should().Be(expected.RequiredAmount);
                requirement.AvailableAmount.Should().Be(expected.AvailableAmount);
                requirement.MissingAmount.Should().Be(expected.MissingAmount);
            }
            result.TotalAvailable.Should().Be(expectedRequirements.Sum(r => r.AvailableAmount));
            result.TotalMissing.Should().Be(expectedRequirements.Sum(r => r.MissingAmount));
            result.TotalRequired.Should().Be(expectedRequirements.Sum(r => r.RequiredAmount));

            //Facility Requirements
            var expectedFacilityRequirements = PrawnSuitBuildPlanOutput.ExpectedFacilityWithNoneInvestory();
            result.FacilityRequirements.Should().HaveCount(expectedFacilityRequirements.Count);
            foreach (var expected in expectedFacilityRequirements)
            {
                var facilityRequirement = result.FacilityRequirements.FirstOrDefault(fr => fr.Name == expected.Name);
                facilityRequirement.Should().NotBeNull();
                facilityRequirement.FacilityId.Should().Be(expected.FacilityId);
                facilityRequirement.Name.Should().Be(expected.Name);
            }
        }

        protected async Task SeedInventoryAsync(params (int componentId, int quantity)[] items)
        {
            foreach (var (componentId, quantity) in items)
            {
                DbContext.QuestComponents.Add(new QuestComponents
                {
                    AmountAquired = quantity,
                    ComponentId = componentId,
                    QuestId = _questId
                });
            }

            await DbContext.SaveChangesAsync();
        }

        private async Task ClearInventorySeed()
        {
            var questComponents = DbContext.QuestComponents.Where(qc => qc.Quest.GameSaveId == _GameSaveId);
            DbContext.QuestComponents.RemoveRange(questComponents);
            await DbContext.SaveChangesAsync();
        }


    }
}