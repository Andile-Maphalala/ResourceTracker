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
