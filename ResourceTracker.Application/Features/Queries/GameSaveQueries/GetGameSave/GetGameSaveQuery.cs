
using MediatR;

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave
{
    public record GetGameSaveQuery(int Id) : IRequest<GetGameSaveResponse>;
}
