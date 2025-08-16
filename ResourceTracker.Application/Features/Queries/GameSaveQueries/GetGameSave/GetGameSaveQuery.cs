
using MediatR;

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave
{
    public class GetGameSaveQuery : IRequest<GetGameSaveResponse>
    {
        public int Id { get; set; }
    }
}
