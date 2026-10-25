using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.RecipeCommands.DeleteRecipe
{
    public class DeleteRecipeValidator : AbstractValidator<DeleteRecipeCommand>
    {
        public DeleteRecipeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
