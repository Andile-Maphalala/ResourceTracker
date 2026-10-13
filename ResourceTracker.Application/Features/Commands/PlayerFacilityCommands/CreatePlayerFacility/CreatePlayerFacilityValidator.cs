using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.CreatePlayerFacility
{
    public class CreatePlayerFacilityValidator : AbstractValidator<CreatePlayerFacilityCommand>
    {
        public CreatePlayerFacilityValidator()
        {
            RuleFor(x => x.ComponentId)
                .GreaterThan(0).WithMessage("Component is required");
            RuleFor(x => x.QuestId)
                .GreaterThan(0).WithMessage("Quest is required");
            RuleFor(x => x.Level)
                .GreaterThanOrEqualTo(1).WithMessage("Level cannot be less than 1");
        }
    }
}
