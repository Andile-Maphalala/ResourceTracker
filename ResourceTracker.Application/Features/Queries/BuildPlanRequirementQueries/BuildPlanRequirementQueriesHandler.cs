
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
            List<BuildPlanComponentRequirementDto> compositeRequirments = new List<BuildPlanComponentRequirementDto>();
            List<BuildPlanComponentRequirementDto> facilityRequirments = new List<BuildPlanComponentRequirementDto>();
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
                foreach (var buildPlanComponent in buildPlanComponetsRequired)
                {

                    if (buildPlanComponent.Component.Type == (int)ComponentTypeEnum.Resource)
                    {
                        requirements.Add(new BuildPlanComponentRequirementDto
                        {
                            ComponentId = buildPlanComponent.Component.Id,
                            ComponentName = buildPlanComponent.Component.Name,
                            RequiredAmount = buildPlanComponent.QuantityNeeded,
                            AvailableAmount = 0,
                            MissingAmount = buildPlanComponent.QuantityNeeded,
                            Type = buildPlanComponent.Component.Type
                        });
                    }
                    else if (buildPlanComponent.Component.Type == (int)ComponentTypeEnum.Composite)
                    {
                        await CalculateComponentRequirements(buildPlanComponent.Component, buildPlanComponent.QuantityNeeded, requirements, request.IgnoreInventory, allInventory, cancellationToken);

                    }
                    else if (buildPlanComponent.Component.Type == (int)ComponentTypeEnum.Facility)
                    {
                        
                    }
                }
            }

            if (request.IgnoreFacilityRequirements)
            {
            }
            if (request.IgnoreInventory)
            {
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

        private async Task CalculateComponentRequirements(Component component, int quantityNeeded, List<BuildPlanComponentRequirementDto> requirements, bool ignoreInventory, List<QuestComponents> Inventory, CancellationToken cancellationToken)
        {
            var recipes = component.Recipes;
            if (recipes.Any())
            {
                var recipe = recipes.First();
                foreach (var recipeComponent in recipe.RecipeComponents)
                {
                    int totalNeeded = recipeComponent.AmountRequired * quantityNeeded;
                    if (recipeComponent.Component.Type == (int)ComponentTypeEnum.Resource)
                    {
                        var existingRequirement = requirements.FirstOrDefault(x => x.ComponentId == recipeComponent.Component.Id);
                        if (existingRequirement != null)
                        {
                            existingRequirement.RequiredAmount += totalNeeded;
                            existingRequirement.MissingAmount += totalNeeded;
                        }
                        else
                        {
                            if(ignoreInventory)
                            {
                                requirements.Add(new BuildPlanComponentRequirementDto
                                {
                                    ComponentId = recipeComponent.Component.Id,
                                    ComponentName = recipeComponent.Component.Name,
                                    RequiredAmount = totalNeeded,
                                    AvailableAmount = 0,
                                    MissingAmount = totalNeeded,
                                    Type = recipeComponent.Component.Type
                                });
                            }
                            else
                            {
                                var existingComponetInInventory = Inventory.Where(z => z.ComponentId == recipeComponent.Component.Id).FirstOrDefault();
                                if (existingComponetInInventory == null)
                                {
                                    requirements.Add(new BuildPlanComponentRequirementDto
                                    {
                                        ComponentId = recipeComponent.Component.Id,
                                        ComponentName = recipeComponent.Component.Name,
                                        RequiredAmount = totalNeeded,
                                        AvailableAmount = 0,
                                        MissingAmount = totalNeeded,
                                        Type = recipeComponent.Component.Type
                                    });
                                }
                                else
                                {
                                    requirements.Add(new BuildPlanComponentRequirementDto
                                    {
                                        ComponentId = recipeComponent.Component.Id,
                                        ComponentName = recipeComponent.Component.Name,
                                        RequiredAmount = totalNeeded,
                                        AvailableAmount = existingComponetInInventory.AmountAquired,
                                        MissingAmount = totalNeeded - existingComponetInInventory.AmountAquired,
                                        Type = recipeComponent.Component.Type
                                    });
                                }
                            }
                            
                        } 
                    }
                    else
                    {
                        await CalculateComponentRequirements(recipeComponent.Component, totalNeeded, requirements, ignoreInventory, Inventory, cancellationToken);
                    }
                    
                }
            }
        }
    }
}
