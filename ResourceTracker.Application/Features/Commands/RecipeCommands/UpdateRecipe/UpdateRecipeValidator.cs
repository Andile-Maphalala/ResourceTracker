using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.RecipeCommands.UpdateRecipe
{
    public class UpdateRecipeValidator : AbstractValidator<UpdateRecipeCommand>
    {
        public UpdateRecipeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.ComponentId)
                .GreaterThan(0).WithMessage("Component is required");
            RuleFor(x => x.AmountMade)
                .GreaterThan(0).WithMessage("Amount made must be greater than 0");
            RuleFor(x => x.RequiredFacilityLevel)
                .GreaterThanOrEqualTo(1).WithMessage("Level cannot be less than 1");

            RuleForEach(x => x.Ingredients).ChildRules(ingredient =>
            {
                ingredient.RuleFor(x => x.ComponentId)
                    .GreaterThan(0).WithMessage("Ingredient component is required");
                ingredient.RuleFor(x => x.AmountRequired)
                    .GreaterThan(0).WithMessage("Ingredient amount must be greater than 0");
            });
        }
    }
}
