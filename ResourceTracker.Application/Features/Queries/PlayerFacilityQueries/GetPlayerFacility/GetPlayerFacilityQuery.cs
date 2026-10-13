using MediatR;

namespace ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacility
{
    public record GetPlayerFacilityQuery(int Id) : IRequest<GetPlayerFacilityResponse>;
}
