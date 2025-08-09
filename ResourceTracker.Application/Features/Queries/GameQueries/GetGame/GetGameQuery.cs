using MediatR;

namespace ResourceTracker.Application.Features.Queries.GameQueries.GetGame
{
    public class GetGameQuery : IRequest<GetGameResponse>
    {
        public int Id { get; set; }
    }
}
