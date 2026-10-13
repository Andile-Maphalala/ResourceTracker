using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.UpdatePlayerFacility
{
    public class UpdatePlayerFacilityValidator : AbstractValidator<UpdatePlayerFacilityCommand>
    {
        public UpdatePlayerFacilityValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.Level)
                .GreaterThanOrEqualTo(1).WithMessage("Level cannot be less than 1");
        }
    }
}
