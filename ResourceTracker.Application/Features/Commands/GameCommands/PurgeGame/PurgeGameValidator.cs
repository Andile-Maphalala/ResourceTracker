using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameCommands.PurgeGame
{
    public class PurgeGameValidator : AbstractValidator<PurgeGameCommand>
    {
        public PurgeGameValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.ConfirmName)
                .NotEmpty().WithMessage("ConfirmName is required and must match the game's name");
        }
    }
}
