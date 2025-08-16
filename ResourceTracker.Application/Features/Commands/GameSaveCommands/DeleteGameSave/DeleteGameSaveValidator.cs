using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands.DeleteGameSave
{
    public class DeleteGameSaveValidator : AbstractValidator<DeleteGameSaveCommand>
    {
        public DeleteGameSaveValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
