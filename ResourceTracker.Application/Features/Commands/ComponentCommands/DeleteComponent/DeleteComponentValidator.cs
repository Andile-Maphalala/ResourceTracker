using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent
{
    public class DeleteComponentValidator : AbstractValidator<DeleteComponentCommand>
    {
        public DeleteComponentValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0)
                .WithMessage("Id is required");
        }
    }
}
