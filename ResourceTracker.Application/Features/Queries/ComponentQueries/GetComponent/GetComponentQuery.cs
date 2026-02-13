using MediatR;

namespace ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent
{
    public record GetComponentQuery(int Id) : IRequest<GetComponentResponse>;
}
