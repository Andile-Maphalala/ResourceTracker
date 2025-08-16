
using MediatR;

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList
{
    public class GetGameSaveListQuery : IRequest<List<GetGameSaveListResponse>>
    {
        public int GameId { get; set; }
    }
}
