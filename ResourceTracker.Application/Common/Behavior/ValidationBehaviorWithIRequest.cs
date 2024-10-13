using ResourceTracker.Application.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace ResourceTracker.Application.Common.Behavior
{
    public sealed class ValidationBehaviorWithIRequest<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviorWithIRequest(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

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

            return await next();
        }
    }
}
