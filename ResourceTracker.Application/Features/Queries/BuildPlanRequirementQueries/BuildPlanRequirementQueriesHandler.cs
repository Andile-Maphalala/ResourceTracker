
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

            // Immutable snapshot to check facility availability (facilities are not consumed)
            var inventorySnapshot = allInventory.ToDictionary(i => i.ComponentId, i => i.AmountAquired);

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
                foreach (var buildPlanComponent in buildPlanComponetsRequired)
                {
                    await CalculateRequirementsRecursive(
                        buildPlanComponent.Component,
                        buildPlanComponent.QuantityNeeded,
                        recipes,
                        requirements,
                        facilityRequirementsMap,
                        workingInventory,
                        inventorySnapshot,
                        request.IncludeInventory,
                        request.IncludeFacilityRequirements,
                        cancellationToken);
                }
                if (request.IncludeFacilityRequirements && facilityRequirementsMap.Any())
                {
                    // Iteratively process facilities until no new facility requirements are discovered.
                    // This handles facilities that themselves require other facilities.
                    var processedFacilityIds = new HashSet<int>();
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
                                inventorySnapshot,
                                request.IncludeInventory,
                                request.IncludeFacilityRequirements,
                                cancellationToken);
                        }
                    }
                }

                
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
            IReadOnlyDictionary<int, int> inventorySnapshot,
            bool includeInventory,
            bool includeFacilityRequirements,
            CancellationToken cancellationToken)
        {
            int availableFromInventory = 0;
            if (includeInventory)
            {
                var inventoryItem = workingInventory.FirstOrDefault(x => x.ComponentId == component.Id);
                availableFromInventory = inventoryItem?.AmountAquired ?? 0;
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

            // Track facility requirement but determine availability from the ORIGINAL snapshot (not the mutated working inventory)
            if (includeFacilityRequirements && recipe.RequiredFacility != null)
            {
                var facilityId = recipe.RequiredFacility.Id;
                if (!facilityRequirementsMap.TryGetValue(facilityId, out var facilityReq))
                {
                    bool isAvailable = inventorySnapshot.TryGetValue(facilityId, out var amt) && amt > 0;

                    facilityReq = new BuildPlanFacilityRequirementDto
                    {
                        FacilityId = facilityId,
                        Name = recipe.RequiredFacility.Name,
                        IsAvailable = isAvailable,
                        RequiredFor = new List<int> { component.Id },
                        ImageUrl = GetPictureUrl(recipe.RequiredFacility.PictureId)
                    };

                    facilityRequirementsMap[facilityId] = facilityReq;
                }
                else
                {
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
                        inventorySnapshot,
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
