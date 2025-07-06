using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame
{
    public class DeleteGameValidator : AbstractValidator<DeleteGameCommand>
    {
        public DeleteGameValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
