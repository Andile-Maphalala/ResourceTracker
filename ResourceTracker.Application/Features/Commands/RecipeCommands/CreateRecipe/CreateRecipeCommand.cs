using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.RecipeCommands.CreateRecipe
{
    public class CreateRecipeCommand : ICommand<CreateRecipeResponse>
    {
        public int ComponentId { get; set; }
        public int AmountMade { get; set; } = 1;
        public int? RequiredFacilityId { get; set; }
        public int RequiredFacilityLevel { get; set; } = 1;
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientDto
    {
        public int ComponentId { get; set; }
        public int AmountRequired { get; set; }
    }
}
