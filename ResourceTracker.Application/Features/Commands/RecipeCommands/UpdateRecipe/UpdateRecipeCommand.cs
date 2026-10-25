using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Features.Commands.RecipeCommands.CreateRecipe;

namespace ResourceTracker.Application.Features.Commands.RecipeCommands.UpdateRecipe
{
    public class UpdateRecipeCommand : ICommand
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public int AmountMade { get; set; } = 1;
        public int? RequiredFacilityId { get; set; }
        public int RequiredFacilityLevel { get; set; } = 1;
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    }
}
