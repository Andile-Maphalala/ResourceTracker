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
                    Type = (int)GetComponentType(componentDto),
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
                    // A station the data names but never declares is a broken file, not a recipe with
                    // no station: importing it as null drops the facility requirement silently and
                    // nothing downstream can tell it was meant to be there. A null station is
                    // legitimate - it means hand-crafted, no station needed.
                    Component? facility = null;
                    if (!string.IsNullOrWhiteSpace(componentDto.Recipe.Station))
                    {
                        facility = components.FirstOrDefault(c => c.Name == componentDto.Recipe.Station);
                        if (facility == null)
                        {
                            throw new BadRequestException($"Station {componentDto.Recipe.Station} required by {componentDto.Name} is not a declared component");
                        }
                    }

                    var recipe = new Recipe
                    {
                        ComponentId = component.Id,
                        AmountMade = componentDto.Recipe.OutputQuantity,
                        RequiredFacilityId = facility?.Id,
                        // 0 in the data means "any level of this station", so it is normalised to the
                        // lowest a facility can be (1) rather than stored as a requirement of zero.
                        RequiredFacilityLevel = componentDto.Recipe.StationLevel > 0 ? componentDto.Recipe.StationLevel : 1
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


        public static ComponentTypeEnum GetComponentType(GameComponentDto component)
        {
            if (string.IsNullOrWhiteSpace(component.Type))
            {
                throw new BadRequestException(
                    $"Component {component.Name} has no type. Expected one of {string.Join(", ", Enum.GetNames<ComponentTypeEnum>())}.");
            }

            if (!Enum.TryParse<ComponentTypeEnum>(component.Type, ignoreCase: true, out var type)
                || !Enum.IsDefined(type))
            {
                throw new BadRequestException(
                    $"Component {component.Name} has unknown type {component.Type}. Expected one of {string.Join(", ", Enum.GetNames<ComponentTypeEnum>())}.");
            }

            return type;
        }
    }
}
