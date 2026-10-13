using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.DeletePlayerFacility
{
    public class DeletePlayerFacilityValidator : AbstractValidator<DeletePlayerFacilityCommand>
    {
        public DeletePlayerFacilityValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
