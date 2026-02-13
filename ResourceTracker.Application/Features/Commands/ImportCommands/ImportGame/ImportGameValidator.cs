
using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.ImportGame
{
    public class ImportGameValidator : AbstractValidator<ImportGameCommand>
    {
        public ImportGameValidator()
        {
            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("GameId is required");
        }
    }
}
