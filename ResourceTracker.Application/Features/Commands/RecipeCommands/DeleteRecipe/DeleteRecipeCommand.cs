using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.RecipeCommands.DeleteRecipe
{
    public record DeleteRecipeCommand(int Id) : ICommand;
}
