
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using Component = ResourceTracker.Domain.Entities.Component;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries
{
    public class BuildPlanRequirementQueriesHandler : IRequestHandler<GetBuildPlanRequirementQuery, GetBuildPlanRequirementsResponse>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public BuildPlanRequirementQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetBuildPlanRequirementsResponse> Handle(GetBuildPlanRequirementQuery request, CancellationToken cancellationToken)
        {
            List<BuildPlanComponentRequirementDto> requirements = new List<BuildPlanComponentRequirementDto>();

            // Use a dictionary for facility requirements for O(1) lookups
            var facilityRequirementsMap = new Dictionary<int, BuildPlanFacilityRequirementDto>();

            //get all componets to build and their recipes in a single query to avoid N+1 issues during recursion
            var buildPlan = await _repo.BuildPlans.Where(x => x.Id == request.BuildPlanId)
                .Include(x => x.GameSave)
                .Include(z => z.BuildPlanQuests)
                .ThenInclude(y => y.Quest)
                .ThenInclude(x => x.QuestComponents)
                .ThenInclude(y => y.Component)
                .Include(z => z.BuildPlanQuests)
                .ThenInclude(y => y.Quest)
                .ThenInclude(y => y.PlayerFacilities)
                .Include(z => z.BuildPlanComponents)
                .ThenInclude(y => y.Component)
                .FirstOrDefaultAsync(cancellationToken);


            if (buildPlan == null)
            {
                throw new NotFoundException(nameof(buildPlan), request.BuildPlanId);
            }

            if(buildPlan.GameSave == null)
            {
                throw new NotFoundException($"Game Save not found for Build Plan {request.BuildPlanId}");
            }

            var recipes = await _repo.Recipes
               .Include(x => x.RequiredFacility)
               .Include(x => x.RecipeComponents)
               .ThenInclude(x => x.Component)
               .Where(x => x.Component.GameId == buildPlan.GameSave.GameId)
               .ToListAsync(cancellationToken);

            var allInventory = buildPlan.BuildPlanQuests.SelectMany(x => x.Quest.QuestComponents).ToList();

            // Facilities are capability, not stock: they live in PlayerFacility and never in the
            // inventory, so they are not deducted from and cannot be consumed. Availability is the
            // best level owned across the plan's locations - the same facility at a second base is
            // a separate row, and a level-4 forge satisfies a level-3 recipe. Absent means 0.
            var ownedFacilityLevels = buildPlan.BuildPlanQuests
                .SelectMany(x => x.Quest.PlayerFacilities)
                .GroupBy(f => f.ComponentId)
                .ToDictionary(g => g.Key, g => g.Max(f => f.Level));

            // Working copy we can mutate while deducting consumed components
            var workingInventory = allInventory
                .Select(i => new QuestComponents
                {
                    ComponentId = i.ComponentId,
                    AmountAquired = i.AmountAquired
                })
                .ToList();

            var buildPlanComponetsRequired = buildPlan.BuildPlanComponents.OrderBy(x => x.Order).ToList();
            if (buildPlanComponetsRequired.Any())
            {
                // A facility asked for as a plan item is walked here, so mark it: the facility pass
                // below would otherwise walk it a second time and bill its build materials twice.
                var processedFacilityIds = new HashSet<int>();

                foreach (var buildPlanComponent in buildPlanComponetsRequired)
                {
                    if (buildPlanComponent.Component.Type == (int)ComponentTypeEnum.Facility)
                        processedFacilityIds.Add(buildPlanComponent.Component.Id);

                    await CalculateRequirementsRecursive(
                        buildPlanComponent.Component,
                        buildPlanComponent.QuantityNeeded,
                        recipes,
                        requirements,
                        facilityRequirementsMap,
                        workingInventory,
                        ownedFacilityLevels,
                        buildPlan.IncludeInventory,
                        buildPlan.IncludeFacilityRequirements,
                        cancellationToken);
                }
                if (buildPlan.IncludeFacilityRequirements && facilityRequirementsMap.Any())
                {
                    // Iteratively process facilities until no new facility requirements are discovered.
                    // This handles facilities that themselves require other facilities.
                    while (true)
                    {
                        // determine which facility ids still need to be fetched/processed
                        var toProcessIds = facilityRequirementsMap.Keys.Except(processedFacilityIds).ToList();
                        if (!toProcessIds.Any())
                            break;

                        // batch query the components for these facilities;
                        // include Recipes, Recipe.RecipeComponents and Recipe.RequiredFacility so recursion can inspect them
                        var facilityComponents = _repo.Components
                            .Where(c => toProcessIds.Contains(c.Id))
                            .Include(c => c.Recipes)
                                .ThenInclude(r => r.RequiredFacility)
                            .ToList();

                        foreach (var facilityComponent in facilityComponents)
                        {
                            // mark as processed to avoid refetching endlessly
                            processedFacilityIds.Add(facilityComponent.Id);

                            // process the facility component as a top-level craft (quantity = 1).
                            // If this processing discovers new facility dependencies they will get added to facilityRequirementsMap,
                            // causing the while loop to iterate again and fetch/process them.
                            await CalculateRequirementsRecursive(
                                facilityComponent,
                                1,
                                recipes,
                                requirements,
                                facilityRequirementsMap,
                                workingInventory,
                                ownedFacilityLevels,
                                buildPlan.IncludeInventory,
                                buildPlan.IncludeFacilityRequirements,
                                cancellationToken);
                        }
                    }
                }

                
            }

            var questNameById = buildPlan.BuildPlanQuests.ToDictionary(bpq => bpq.QuestId, bpq => bpq.Quest.Name);

            foreach (var req in requirements)
            {
                req.Locations = allInventory
                    .Where(i => i.ComponentId == req.ComponentId)
                    .Select(i => new ComponentLocationDto
                    {
                        QuestComponentId = i.Id,
                        QuestId = i.QuestId,
                        QuestName = questNameById.TryGetValue(i.QuestId, out var name) ? name : "Unknown",
                        Quantity = i.AmountAquired
                    })
                    .ToList();
            }

            var response = new GetBuildPlanRequirementsResponse
            {
                BuildPlanId = buildPlan.Id,
                BuildPlanName = buildPlan.Name,
                Requirements = requirements,
                TotalAvailable = requirements.Sum(x => x.AvailableAmount),
                TotalMissing = requirements.Sum(x => x.MissingAmount),
                TotalRequired = requirements.Sum(x => x.RequiredAmount),
                FacilityRequirements = facilityRequirementsMap.Values.ToList()
            };
            return response;

        }

        private async Task CalculateRequirementsRecursive(
            Component component,
            int quantityNeeded,
            List<Recipe> gameRecipes,
            List<BuildPlanComponentRequirementDto> requirements,
            Dictionary<int, BuildPlanFacilityRequirementDto> facilityRequirementsMap,
            List<QuestComponents> workingInventory,
            IReadOnlyDictionary<int, int> ownedFacilityLevels,
            bool includeInventory,
            bool includeFacilityRequirements,
            CancellationToken cancellationToken)
        {
            // A facility the player already owns among the plan's locations needs nothing gathered,
            // and must not land in the requirements table: that table is stock, and its rows are
            // what Consume acts on. Facility status is reported through FacilityRequirements.
            if (component.Type == (int)ComponentTypeEnum.Facility && ownedFacilityLevels.ContainsKey(component.Id))
            {
                return;
            }

            int availableFromInventory = 0;
            if (includeInventory)
            {
                availableFromInventory = workingInventory.Where(x => x.ComponentId == component.Id).Sum(x => x.AmountAquired);
            }

            int stillNeeded = Math.Max(0, quantityNeeded - availableFromInventory);
            if (stillNeeded == 0)
            {
                AddOrUpdateRequirement(requirements, component, quantityNeeded, availableFromInventory);
                DeductFromInventory(workingInventory, component.Id, quantityNeeded);
                return;
            }
            else if (availableFromInventory > 0 && component.Type == (int)ComponentTypeEnum.Composite)
            {
                // Partially satisfied from inventory
                AddOrUpdateRequirement(requirements, component, quantityNeeded, availableFromInventory);
                DeductFromInventory(workingInventory, component.Id, availableFromInventory);
            }

            if (component.Type == (int)ComponentTypeEnum.Resource)
            {
                // Raw material, just add to requirements (resources are leaf nodes)
                AddOrUpdateRequirement(requirements, component, quantityNeeded, availableFromInventory);
                DeductFromInventory(workingInventory, component.Id, quantityNeeded);
                return;
            }

            var recipes = gameRecipes.Where(r => r.ComponentId == component.Id).ToList();
            if (recipes == null || !recipes.Any())
            {
                // No recipe exists (shouldn't happen for composites/facilities)
                return;
            }

            var recipe = recipes.First();

            // Track the facility requirement against what the player owns, not the working
            // inventory: a facility is never deducted here because it is never consumed.
            if (includeFacilityRequirements && recipe.RequiredFacility != null)
            {
                var facilityId = recipe.RequiredFacility.Id;
                var requiredLevel = recipe.RequiredFacilityLevel;
                var ownedLevel = ownedFacilityLevels.TryGetValue(facilityId, out var level) ? level : 0;

                if (!facilityRequirementsMap.TryGetValue(facilityId, out var facilityReq))
                {
                    facilityReq = new BuildPlanFacilityRequirementDto
                    {
                        FacilityId = facilityId,
                        Name = recipe.RequiredFacility.Name,
                        RequiredLevel = requiredLevel,
                        OwnedLevel = ownedLevel,
                        IsAvailable = ownedLevel >= requiredLevel,
                        RequiredFor = new List<int> { component.Id },
                        ImageUrl = GetPictureUrl(recipe.RequiredFacility.PictureId)
                    };

                    facilityRequirementsMap[facilityId] = facilityReq;
                }
                else
                {
                    // Several recipes can need the same station at different levels. A level-4 forge
                    // satisfies a level-3 ask, so the facility requirement is the MAX of the levels
                    // demanded - never the sum, and never whichever recipe happened to be walked first.
                    if (requiredLevel > facilityReq.RequiredLevel)
                        facilityReq.RequiredLevel = requiredLevel;

                    facilityReq.OwnedLevel = ownedLevel;
                    facilityReq.IsAvailable = ownedLevel >= facilityReq.RequiredLevel;

                    if (!facilityReq.RequiredFor.Contains(component.Id))
                        facilityReq.RequiredFor.Add(component.Id);
                }
            }

            // Calculate how many to craft (batches)
            int toCraft = Math.Max(0, stillNeeded);
            int batchesToCraft = (int)Math.Ceiling((double)toCraft / recipe.AmountMade);

            // Process each ingredient
            if(recipe.RecipeComponents != null)
            {
                foreach (var recipeComponent in recipe.RecipeComponents)
                {
                    int ingredientNeeded = recipeComponent.AmountRequired * batchesToCraft;

                    await CalculateRequirementsRecursive(
                        recipeComponent.Component,
                        ingredientNeeded,
                        gameRecipes,
                        requirements,
                        facilityRequirementsMap,
                        workingInventory,
                        ownedFacilityLevels,
                        includeInventory,
                        includeFacilityRequirements,
                        cancellationToken);
                }
            }
        }


        private void AddOrUpdateRequirement(List<BuildPlanComponentRequirementDto> requirements, Component component, int required, int available)
        {
            var existing = requirements.FirstOrDefault(x => x.ComponentId == component.Id);

            if (existing != null)
            {
                existing.RequiredAmount += required;
                existing.AvailableAmount += available;
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
                    Type = component.Type,
                    ImageUrl = GetPictureUrl(component.PictureId)
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

        private string? GetPictureUrl(int? pictureId)
        {
            if(pictureId == null || (pictureId == 0))
            {
                return null;
            }
            var path = _repo.Pictures.FirstOrDefault(x => x.Id == pictureId)?.Path;
            return ImageHelper.GetFileUrl(path ?? string.Empty, _baseUrl);
        }
    }
}
