using FluentValidation;
using ResourceTracker.Application.Common.Exceptions;

namespace ResourceTracker.Application.Common.Behavior
{
    public class ValidationBehavior<TRequest>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task ValidateAsync(TRequest request, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return;

            var context = new ValidationContext<TRequest>(request);

            var errors = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .Select(x => x.ErrorMessage)
                .Distinct()
                .ToArray();

            if (errors.Any())
                throw new BadRequestException(errors);
        }
    }

}
