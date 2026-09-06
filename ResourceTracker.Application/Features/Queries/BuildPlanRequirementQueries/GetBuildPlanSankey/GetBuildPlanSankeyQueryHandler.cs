

using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanSankey
{
    public class GetBuildPlanSankeyQueryHandler(IResourceTrackerRepository repo) : IRequestHandler<GetBuildPlanSankeyQuery, GetBuildPlanSankeyResponse>
    {
        public async Task<GetBuildPlanSankeyResponse> Handle(GetBuildPlanSankeyQuery request, CancellationToken cancellationToken)
        {
            var buildPlan = await repo.BuildPlans.Where(x => x.Id == request.BuildPlanId)
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

            if (buildPlan.GameSave == null)
            {
                throw new NotFoundException($"Game Save not found for Build Plan {request.BuildPlanId}");
            }

            var recipes = await repo.Recipes
                .Include(x => x.RequiredFacility)
                .Include(x => x.RecipeComponents)
                    .ThenInclude(x => x.Component)
                .Where(x => x.Component.GameId == buildPlan.GameSave.GameId)
                .ToListAsync(cancellationToken);

            var allInventory = buildPlan.BuildPlanQuests.SelectMany(x => x.Quest.QuestComponents).ToList();

            var workingInventory = allInventory
                .Select(i => new QuestComponents
                {
                    ComponentId = i.ComponentId,
                    AmountAquired = i.AmountAquired
                })
                .ToList();

            var edgeMap = new Dictionary<(string Source, string Target), int>();
            var stations = new Dictionary<int, ComponentFacilityDto>();

            var topLevel = buildPlan.BuildPlanComponents.OrderBy(x => x.Order).ToList();
            foreach (var buildPlanComponent in topLevel)
            {
                CalculateSankeyRecursive(
                    buildPlanComponent.Component,
                    buildPlanComponent.QuantityNeeded,
                    parentName: null, // top-level requirement is a sink, nothing consumes it further
                    recipes,
                    edgeMap,
                    stations,
                    workingInventory,
                    buildPlan.IncludeInventory);
            }

            var response = new GetBuildPlanSankeyResponse
            {
                BuildPlanId = buildPlan.Id,
                BuildPlanName = buildPlan.Name,
                Edges = edgeMap
                    .Where(kv => kv.Value > 0)
                    .Select(kv => new SankeyEdgeDto { Source = kv.Key.Source, Target = kv.Key.Target, Value = kv.Value })
                    .ToList(),
                CraftingStations = stations.Values.ToList()
            };

            return response;
        }

        private void CalculateSankeyRecursive(
            Component component,
            int quantityNeeded,
            string parentName,
            List<Recipe> gameRecipes,
            Dictionary<(string Source, string Target), int> edgeMap,
            Dictionary<int, ComponentFacilityDto> stations,
            List<QuestComponents> workingInventory,
            bool includeInventory)
        {
            // The total flow of this component into whatever needed it.
            if (parentName != null)
            {
                AddEdge(edgeMap, component.Name, parentName, quantityNeeded);
            }

            int availableFromInventory = 0;
            if (includeInventory)
            {
                availableFromInventory = workingInventory.Where(x => x.ComponentId == component.Id).Sum(x => x.AmountAquired);
                availableFromInventory = Math.Min(availableFromInventory, quantityNeeded);
                if (availableFromInventory > 0)
                {
                    AddEdge(edgeMap, "Inventory", component.Name, availableFromInventory);
                    DeductFromInventory(workingInventory, component.Id, availableFromInventory);
                }
            }

            int stillNeeded = Math.Max(0, quantityNeeded - availableFromInventory);
            if (stillNeeded == 0)
            {
                return; // fully covered by stock, no crafting/gathering edges needed
            }

            if (component.Type == (int)ComponentTypeEnum.Resource)
            {
                // Raw material with no recipe - whatever isn't already in inventory still needs
                // to be gathered from the world. Virtual source node keeps the node's inflow/outflow balanced.
                AddEdge(edgeMap, "To Gather", component.Name, stillNeeded);
                return;
            }

            var recipe = gameRecipes.FirstOrDefault(r => r.ComponentId == component.Id);
            if (recipe == null)
            {
                return; // shouldn't happen for composites/facilities, mirrors existing guard
            }

            if (recipe.RequiredFacility != null && !stations.ContainsKey(component.Id))
            {
                stations[component.Id] = new ComponentFacilityDto
                {
                    ComponentName = component.Name,
                    FacilityName = recipe.RequiredFacility.Name
                };
            }

            int batchesToCraft = (int)Math.Ceiling((double)stillNeeded / recipe.AmountMade);

            if (recipe.RecipeComponents != null)
            {
                foreach (var recipeComponent in recipe.RecipeComponents)
                {
                    int ingredientNeeded = recipeComponent.AmountRequired * batchesToCraft;

                    CalculateSankeyRecursive(
                        recipeComponent.Component,
                        ingredientNeeded,
                        component.Name, // this component is what consumes the ingredient
                        gameRecipes,
                        edgeMap,
                        stations,
                        workingInventory,
                        includeInventory);
                }
            }
        }

        private void AddEdge(Dictionary<(string Source, string Target), int> edgeMap, string source, string target, int value)
        {
            if (value <= 0) return;

            var key = (source, target);
            edgeMap[key] = edgeMap.TryGetValue(key, out var existing) ? existing + value : value;
        }

        private void DeductFromInventory(List<QuestComponents> inventory, int componentId, int amountToDeduct)
        {
            var item = inventory.FirstOrDefault(x => x.ComponentId == componentId);
            if (item != null)
            {
                item.AmountAquired = Math.Max(0, item.AmountAquired - amountToDeduct);
            }
        }
    }
}
