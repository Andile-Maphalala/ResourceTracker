
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries
{
    public class BuildPlanRequirementQueriesHandler : IRequestHandler<GetBuildPlanRequirementQuery, GetBuildPlanRequirementsResponse>
    {
        private readonly IResourceTrackerRepository _repo;

        public BuildPlanRequirementQueriesHandler(IResourceTrackerRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetBuildPlanRequirementsResponse> Handle(GetBuildPlanRequirementQuery request, CancellationToken cancellationToken)
        {
            List<BuildPlanComponentRequirementDto> requirements = new List<BuildPlanComponentRequirementDto>();
            List<BuildPlanFacilityRequirementDto> facilityRequirements = new List<BuildPlanFacilityRequirementDto>();

            var buildPlan = await _repo.BuildPlans.Where(x => x.Id == request.BuildPlanId)
                .Include(z => z.BuildPlanQuests)
                .ThenInclude(y => y.Quest)
                .ThenInclude(x => x.QuestComponents)
                .ThenInclude(y => y.Component)
                .ThenInclude(x => x.Recipes)
                .Include(z => z.BuildPlanComponents)
                .ThenInclude(y => y.Component)
                .ThenInclude(x => x.Recipes)
                .FirstOrDefaultAsync(cancellationToken);

            var allInventory = buildPlan.BuildPlanQuests.SelectMany(x => x.Quest.QuestComponents).ToList();
            var buildPlanComponetsRequired = buildPlan.BuildPlanComponents.ToList();
            if (buildPlanComponetsRequired.Any())
            {
                foreach (var buildPlanComponent in buildPlan.BuildPlanComponents)
                {
                    await CalculateRequirementsRecursive(
                        buildPlanComponent.Component,
                        buildPlanComponent.QuantityNeeded,
                        requirements,
                        facilityRequirements,
                        allInventory,
                        request.IgnoreInventory,
                        request.IgnoreFacilityRequirements,
                        new HashSet<int>(),
                        cancellationToken);
                }
            }

            var response = new GetBuildPlanRequirementsResponse
            {
                BuildPlanId = buildPlan.Id,
                BuildPlanName = buildPlan.Name,
                Requirements = requirements,
                TotalAvailable = requirements.Sum(x => x.AvailableAmount),
                TotalMissing = requirements.Sum(x => x.MissingAmount),
                TotalRequired = requirements.Sum(x => x.RequiredAmount)
            };

            return response;
        }

        private async Task CalculateRequirementsRecursive(
    Component component,
    int quantityNeeded,
    List<BuildPlanComponentRequirementDto> requirements,
    List<BuildPlanFacilityRequirementDto> facilityRequirements,
    List<QuestComponents> inventory,
    bool ignoreInventory,
    bool ignoreFacilityRequirements,
    HashSet<int> visitedComponents,
    CancellationToken cancellationToken)
        {
            // Prevent infinite recursion
            if (!visitedComponents.Add(component.Id))
                return;

            int availableFromInventory = 0;
            if (!ignoreInventory)
            {
                var inventoryItem = inventory.FirstOrDefault(x => x.ComponentId == component.Id);
                availableFromInventory = inventoryItem?.AmountAquired ?? 0;
            }

            int stillNeeded = Math.Max(0, quantityNeeded - availableFromInventory);
            if (stillNeeded == 0)
            {
                AddOrUpdateRequirement(requirements, component, quantityNeeded, availableFromInventory);
                DeductFromInventory(inventory, component.Id, quantityNeeded);
                return;
            }
            else if (availableFromInventory > 0 && component.Type == (int)ComponentTypeEnum.Composite)
            {
                // Partially satisfied from inventory
                AddOrUpdateRequirement(requirements, component, quantityNeeded, availableFromInventory);
                DeductFromInventory(inventory, component.Id, availableFromInventory);
            }


            if (component.Type == (int)ComponentTypeEnum.Resource)
            {
                // Raw material, just add to requirements
                AddOrUpdateRequirement(requirements, component, quantityNeeded, availableFromInventory);
                DeductFromInventory(inventory, component.Id, quantityNeeded);
                return;
            }

            var recipes = component.Recipes.ToList();
            if (!recipes.Any())
            {
                // No recipe exists (shouldn't happen for composites/facilities)
                return;
            }

            var recipe = recipes.First();

            // Check facility requirement if not ignoring
            //if (!ignoreFacilityRequirements && recipe.RequiredFacility != null)
            //{
            //    var facilityReq = new BuildPlanFacilityRequirementDto
            //    {
            //        FacilityId = recipe.RequiredFacility.Id,
            //        FacilityName = recipe.RequiredFacility.Name,
            //        IsAvailable = await CheckFacilityAvailable(recipe.RequiredFacility.Id, inventory),
            //        RequiredFor = component.Name
            //    };

            //    if (!facilityRequirements.Any(f => f.FacilityId == facilityReq.FacilityId))
            //    {
            //        facilityRequirements.Add(facilityReq);
            //    }
            //}

            /// Calculate how many we need to CRAFT (not how many total we need)
            int toCraft = Math.Max(0, stillNeeded);
            int batchesToCraft = (int)Math.Ceiling((double)toCraft / recipe.AmountMade);

            // Process each ingredient
            foreach (var recipeComponent in recipe.RecipeComponents)
            {
                int ingredientNeeded = recipeComponent.AmountRequired * batchesToCraft;

                await CalculateRequirementsRecursive(
                    recipeComponent.Component,
                    ingredientNeeded,
                    requirements,
                    facilityRequirements,
                    inventory,
                    ignoreInventory,
                    ignoreFacilityRequirements,
                    visitedComponents,
                    cancellationToken);
            }

        }


        private void AddOrUpdateRequirement(List<BuildPlanComponentRequirementDto> requirements, Component component, int required, int available)
        {
            var existing = requirements.FirstOrDefault(x => x.ComponentId == component.Id);

            if (existing != null)
            {
                existing.RequiredAmount += required;
                existing.AvailableAmount = Math.Max(existing.AvailableAmount, available);
                existing.MissingAmount = Math.Max(0, existing.RequiredAmount - existing.AvailableAmount);
            }
            else
            {
                requirements.Add(new BuildPlanComponentRequirementDto
                {
                    ComponentId = component.Id,
                    ComponentName = component.Name,
                    RequiredAmount = required,
                    AvailableAmount = available,
                    MissingAmount = Math.Max(0, required - available),
                    Type = component.Type
                });
            }
        }

        private void DeductFromInventory(List<QuestComponents> inventory, int componentId, int amountToDeduct)
        {
            var inventoryItem = inventory.FirstOrDefault(x => x.ComponentId == componentId);
            if (inventoryItem != null)
            {
                inventoryItem.AmountAquired = Math.Max(0, inventoryItem.AmountAquired - amountToDeduct);
            }
        }
    }
}
