
using MediatR;

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList
{
    public record GetGameSaveListQuery(int GameId) : IRequest<List<GetGameSaveListResponse>>;
}
