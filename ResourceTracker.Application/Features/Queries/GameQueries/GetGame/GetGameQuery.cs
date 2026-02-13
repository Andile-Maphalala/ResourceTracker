using MediatR;

namespace ResourceTracker.Application.Features.Queries.GameQueries.GetGame
{
    public record GetGameQuery(int Id) : IRequest<GetGameResponse>;
}
