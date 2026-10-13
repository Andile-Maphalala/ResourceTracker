using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanRequirementTests
{
    /// <summary>
    /// Facility availability comes from PlayerFacility, not inventory, and is compared with >=.
    /// Nothing else in the suite seeds a PlayerFacility, so without these the whole satisfaction
    /// path - the point of the facility-levels change - is only verified by construction.
    /// </summary>
    public class BuildPlanRequirementFacilityLevelTests : IntegrationTestBase
    {
        private int _gameId;
        private int _gameSaveId;
        private int _questAId;
        private int _questBId;
        private int _buildPlanId;

        private int _ironId;
        private int _workbenchId;
        private int _plateId;
        private int _bracketId;

        public BuildPlanRequirementFacilityLevelTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            SetupNonAdminUser();

            _gameId = await AddGameRecord();
            _gameSaveId = await AddGameSaveRecord(_gameId);
            _questAId = await AddQuestRecord(_gameSaveId, "Base A");
            _questBId = await AddQuestRecord(_gameSaveId, "Base B");

            _ironId = await AddComponentRecord("Iron", ComponentTypeEnum.Resource);
            _workbenchId = await AddComponentRecord("Workbench", ComponentTypeEnum.Facility);
            _plateId = await AddComponentRecord("Plate", ComponentTypeEnum.Composite);
            _bracketId = await AddComponentRecord("Bracket", ComponentTypeEnum.Composite);

            // The Workbench is built from Iron. Plate and Bracket are both crafted at the Workbench
            // but demand different levels of it, so a plan asking for both pins the MAX rule.
            await AddRecipeRecord(_workbenchId, facilityId: null, facilityLevel: 1, amountMade: 1, (_ironId, 3));
            await AddRecipeRecord(_plateId, facilityId: _workbenchId, facilityLevel: 4, amountMade: 1, (_ironId, 2));
            await AddRecipeRecord(_bracketId, facilityId: _workbenchId, facilityLevel: 2, amountMade: 1, (_ironId, 1));

            _buildPlanId = await AddBuildPlanRecord();
            await LinkQuestToBuildPlan(_questAId);

            // Order matters: the lower requirement is walked first, so the higher one exercises
            // the "raise the max" branch rather than setting it on first sight.
            await AddBuildPlanComponentRecord(_bracketId, order: 1);
            await AddBuildPlanComponentRecord(_plateId, order: 2);
        }

        [Fact]
        public async Task Facility_OwnedAtExactlyTheRequiredLevel_IsAvailable()
        {
            await AddPlayerFacility(_workbenchId, _questAId, level: 4);

            var workbench = await GetWorkbenchRequirement();

            workbench.RequiredLevel.Should().Be(4);
            workbench.OwnedLevel.Should().Be(4);
            workbench.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task Facility_OwnedAboveTheRequiredLevel_IsAvailable()
        {
            // The >= in one test: a level-6 workbench satisfies a recipe wanting level 4.
            await AddPlayerFacility(_workbenchId, _questAId, level: 6);

            var workbench = await GetWorkbenchRequirement();

            workbench.RequiredLevel.Should().Be(4);
            workbench.OwnedLevel.Should().Be(6);
            workbench.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task Facility_OwnedBelowTheRequiredLevel_IsNotAvailable()
        {
            // Built, but not good enough - the middle state a bool could not express.
            await AddPlayerFacility(_workbenchId, _questAId, level: 3);

            var workbench = await GetWorkbenchRequirement();

            workbench.RequiredLevel.Should().Be(4);
            workbench.OwnedLevel.Should().Be(3);
            workbench.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public async Task Facility_NotOwnedAnywhere_ReportsLevelZeroAndIsNotAvailable()
        {
            var workbench = await GetWorkbenchRequirement();

            workbench.RequiredLevel.Should().Be(4);
            workbench.OwnedLevel.Should().Be(0);
            workbench.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public async Task Facility_DemandedAtTwoLevels_IsReportedOnceAtTheHighest()
        {
            var result = await GetRequirements();

            var workbenches = result.FacilityRequirements.Where(f => f.FacilityId == _workbenchId).ToList();
            workbenches.Should().ContainSingle();
            workbenches.Single().RequiredLevel.Should().Be(4);
            workbenches.Single().RequiredFor.Should().Contain(new[] { _plateId, _bracketId });
        }

        [Fact]
        public async Task Facility_RequiredLevel_FollowsThePlanNotTheData()
        {
            // Drop the level-4 recipe from the plan and only the level-2 demand is left, so the
            // requirement cannot be a constant and cannot be the first recipe walked either.
            await RemoveBuildPlanComponent(_plateId);

            var workbench = await GetWorkbenchRequirement();

            workbench.RequiredLevel.Should().Be(2);
        }

        [Fact]
        public async Task Facility_OwnedAtAnotherLinkedLocation_IsAvailable()
        {
            // A facility at a second base is its own row; the plan's scope covers every linked quest.
            await LinkQuestToBuildPlan(_questBId);
            await AddPlayerFacility(_workbenchId, _questBId, level: 4);

            var workbench = await GetWorkbenchRequirement();

            workbench.OwnedLevel.Should().Be(4);
            workbench.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task Facility_BestLevelAcrossLocations_Wins()
        {
            await LinkQuestToBuildPlan(_questBId);
            await AddPlayerFacility(_workbenchId, _questAId, level: 2);
            await AddPlayerFacility(_workbenchId, _questBId, level: 5);

            var workbench = await GetWorkbenchRequirement();

            workbench.OwnedLevel.Should().Be(5);
            workbench.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task FacilityAsPlanItem_WhenOwned_IsNotListedAsAMissingRequirement()
        {
            // Asking for a workbench you already have must not bill you for a second one, and must
            // not appear in the requirements table at all - that table is stock and feeds Consume.
            await PlanOnlyTheWorkbench();
            await AddPlayerFacility(_workbenchId, _questAId, level: 1);

            var result = await GetRequirements();

            result.Requirements.Should().BeEmpty();
            result.TotalMissing.Should().Be(0);
        }

        [Fact]
        public async Task FacilityAsPlanItem_WhenNotOwned_ListsItsBuildMaterials()
        {
            await PlanOnlyTheWorkbench();

            var result = await GetRequirements();

            var iron = result.Requirements.Single(r => r.ComponentId == _ironId);
            iron.RequiredAmount.Should().Be(3);
            iron.MissingAmount.Should().Be(3);
        }

        [Fact]
        public async Task FacilityThatIsAlsoACraftingStation_IsBilledForOnce()
        {
            // The plan wants a Workbench and also crafts at one. Not owned, so the walk expands the
            // Workbench's own build cost - and the facility pass must not expand it a second time.
            await RemoveAllBuildPlanComponents();
            await AddBuildPlanComponentRecord(_workbenchId, order: 1);
            await AddBuildPlanComponentRecord(_plateId, order: 2);

            var result = await GetRequirements();

            // Iron 3 to build the Workbench, plus Iron 2 for the Plate. Walking the Workbench again
            // as a required station would bill that 3 a second time.
            result.Requirements.Single(r => r.ComponentId == _ironId).RequiredAmount.Should().Be(5);
        }

        #region helpers

        private async Task<GetBuildPlanRequirementsResponse> GetRequirements()
        {
            return await Sender.Send(new GetBuildPlanRequirementQuery(_buildPlanId), CancellationToken.None);
        }

        private async Task<BuildPlanFacilityRequirementDto> GetWorkbenchRequirement()
        {
            var result = await GetRequirements();
            var workbench = result.FacilityRequirements.FirstOrDefault(f => f.FacilityId == _workbenchId);
            workbench.Should().NotBeNull("the plan crafts at the Workbench, so it must be reported");
            return workbench!;
        }

        private async Task RemoveAllBuildPlanComponents()
        {
            var items = await DbContext.BuildPlanComponents
                .Where(x => x.BuildPlanId == _buildPlanId)
                .ToListAsync();
            DbContext.BuildPlanComponents.RemoveRange(items);
            await DbContext.SaveChangesAsync();
        }

        private async Task PlanOnlyTheWorkbench()
        {
            await RemoveAllBuildPlanComponents();
            await AddBuildPlanComponentRecord(_workbenchId, order: 1);
        }

        private async Task RemoveBuildPlanComponent(int componentId)
        {
            var item = await DbContext.BuildPlanComponents
                .FirstAsync(x => x.BuildPlanId == _buildPlanId && x.ComponentId == componentId);
            DbContext.BuildPlanComponents.Remove(item);
            await DbContext.SaveChangesAsync();
        }

        private async Task<int> AddGameRecord()
        {
            var game = new Game { Name = "Facility Level Game", Description = "Facility level tests" };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

        private async Task<int> AddGameSaveRecord(int gameId)
        {
            var gameSave = new GameSave
            {
                Name = "Facility Level Save",
                Description = "Facility level tests",
                GameId = gameId,
                Created = DateTime.UtcNow,
                UserId = NonAdminUserId
            };
            DbContext.GameSaves.Add(gameSave);
            await DbContext.SaveChangesAsync();
            return gameSave.Id;
        }

        private async Task<int> AddQuestRecord(int gameSaveId, string name)
        {
            var quest = new Quest
            {
                Name = name,
                Description = "Facility level tests",
                Location = name,
                GameSaveId = gameSaveId
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            return quest.Id;
        }

        private async Task<int> AddComponentRecord(string name, ComponentTypeEnum type)
        {
            var component = new Component
            {
                Name = name,
                Description = "Facility level tests",
                GameId = _gameId,
                Type = (int)type
            };
            DbContext.Components.Add(component);
            await DbContext.SaveChangesAsync();
            return component.Id;
        }

        private async Task AddRecipeRecord(
            int componentId,
            int? facilityId,
            int facilityLevel,
            int amountMade,
            params (int componentId, int amount)[] ingredients)
        {
            var recipe = new Recipe
            {
                ComponentId = componentId,
                RequiredFacilityId = facilityId,
                RequiredFacilityLevel = facilityLevel,
                AmountMade = amountMade,
                RecipeComponents = ingredients.Select(i => new RecipeComponent
                {
                    ComponentId = i.componentId,
                    AmountRequired = i.amount
                }).ToList()
            };

            DbContext.Recipes.Add(recipe);
            await DbContext.SaveChangesAsync();
        }

        private async Task<int> AddBuildPlanRecord()
        {
            var buildPlan = new BuildPlan
            {
                Name = "Facility Level Plan",
                Description = "Facility level tests",
                GameSaveId = _gameSaveId,
                IncludeInventory = true,
                IncludeFacilityRequirements = true
            };
            DbContext.BuildPlans.Add(buildPlan);
            await DbContext.SaveChangesAsync();
            return buildPlan.Id;
        }

        private async Task LinkQuestToBuildPlan(int questId)
        {
            DbContext.BuildPlanQuests.Add(new BuildPlanQuest
            {
                BuildPlanId = _buildPlanId,
                QuestId = questId
            });
            await DbContext.SaveChangesAsync();
        }

        private async Task AddBuildPlanComponentRecord(int componentId, int order)
        {
            DbContext.BuildPlanComponents.Add(new BuildPlanComponent
            {
                Order = order,
                QuantityNeeded = 1,
                ComponentId = componentId,
                BuildPlanId = _buildPlanId
            });
            await DbContext.SaveChangesAsync();
        }

        private async Task AddPlayerFacility(int componentId, int questId, int level)
        {
            // PlayerFacility has no DbSet on the context - like RecipeComponent it is reached
            // through the generic Set<T>(), which is how the repository reads it too.
            DbContext.Set<PlayerFacility>().Add(new PlayerFacility
            {
                ComponentId = componentId,
                QuestId = questId,
                GameSaveId = _gameSaveId,
                Level = level
            });
            await DbContext.SaveChangesAsync();
        }

        #endregion
    }
}
