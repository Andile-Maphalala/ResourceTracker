using MediatR;
using ResourceTracker.Application.Models.Enums;

namespace ResourceTracker.Application.Features.Queries.PictureQueries.GetPictureList
{
    public class GetPictureListQuery : IRequest<List<GetPictureListResponse>>
    {
        public ImageUploadTypeEnum? ImageUploadType { get; set; }
    }
}
