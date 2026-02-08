using MediatR;

namespace ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent
{
    public class GetComponentQuery : IRequest<GetComponentResponse>
    {
        public int Id { get; set; }
    }
}
