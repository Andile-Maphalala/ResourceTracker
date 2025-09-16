using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Services
{
    public static class ImportService
    {
        public static List<Component> ImportComponents(GameDataDto data, int gameId)
        {
            var components = new List<Component>();
            foreach (var componentDto in data.Components)
            {
                var component = new Component
                {
                    Name = componentDto.Name,
                    Description = componentDto.Description ?? "",
                    Type = (int)GetComponentType(componentDto, data.Components),
                    GameId = gameId
                };
                components.Add(component);
            }

            return components;

        }

        public static List<Recipe> ImportRecipes(GameDataDto data, int gameId, List<Component> components, CancellationToken cancellationToken)
        {
            List<Recipe> recipes = new List<Recipe>();
            foreach (var componentDto in data.Components)
            {
                if (componentDto.Recipe != null)
                {
                    var component = components.FirstOrDefault(c => c.Name == componentDto.Name);
                    if (component == null)
                    {
                        throw new BadRequestException($"Component {componentDto.Name} not found");
                    }
                    var facility = components.FirstOrDefault(c => c.Name == componentDto.Recipe.Station);

                    var recipe = new Recipe
                    {
                        ComponentId = component.Id,
                        AmountMade = componentDto.Recipe.OutputQuantity,
                        RequiredFacilityId = facility?.Id,
                    };
                    if(componentDto.Recipe.Requirements.Any())
                    {
                        recipe.RecipeComponents = new List<RecipeComponent>();
                    }
                    foreach (var ingredient in componentDto.Recipe.Requirements)
                    {
                        var ingredientComponent = components.FirstOrDefault(c => c.Name == ingredient.Compononent);
                        if (ingredientComponent == null)
                        {
                            throw new BadRequestException($"Ingredient component {ingredient.Compononent} not found");
                        }
                        recipe.RecipeComponents.Add(new RecipeComponent
                        {
                            ComponentId = ingredientComponent.Id,
                            AmountRequired = ingredient.Quantity
                        });
                    }
                    recipes.Add(recipe);
                }
            }

            return recipes;
        }


        public static ComponentTypeEnum GetComponentType(GameComponentDto component, List<GameComponentDto> data)
        {
            if (component.RawMaterial)
            {
                return ComponentTypeEnum.Resource;
            }
            else
            {
                var stations = GetStations(data);
                if (stations.Contains(component.Name))
                {
                    return ComponentTypeEnum.Facility;
                }
                else
                {
                    return ComponentTypeEnum.Composite;
                }
            }
        }


        private static List<string> GetStations(List<GameComponentDto> data)
        {
            var stations = data
                .Where(c => c.Recipe != null)
                .Select(c => c.Recipe!.Station)
                .Distinct()
                .ToList();
            return stations;
        }
    }
}
