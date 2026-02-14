using MediatR;

namespace ResourceTracker.Application.Features.Queries.PictureQueries.GetPicture
{
    public record GetPictureQuery(int Id) : IRequest<GetPictureResponse>;
}
