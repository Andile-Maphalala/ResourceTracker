

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.PictureQueries.GetPicture;
using ResourceTracker.Application.Features.Queries.PictureQueries.GetPictureList;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Queries.PictureQueries
{
    public class PictureQueriesHandler : IRequestHandler<GetPictureQuery, GetPictureResponse>,
                                         IRequestHandler<GetPictureListQuery, List<GetPictureListResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public PictureQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl ?? throw new InvalidOperationException("BaseUrl is missing.");
        }
        public async Task<GetPictureResponse> Handle(GetPictureQuery request, CancellationToken cancellationToken)
        {
            var item = await _repo.Pictures.
                FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (item == null)
                throw new NotFoundException(nameof(Picture), request.Id);


            var response = new GetPictureResponse
            {
                Id = item.Id,
                Name = item.Name,
                AltText = item.AltText,
                Size = item.Size,
                Url = item.GetFileUrl(_baseUrl),
                ImageUploadType = item.LinkedEntityType,
                ImageUploadTypeName = GetUploadTypeName(item.LinkedEntityType),
            };
            return response;

        }

        public async Task<List<GetPictureListResponse>> Handle(GetPictureListQuery request, CancellationToken cancellationToken)
        {
            var query = _repo.Pictures.AsQueryable();

            if (request.ImageUploadType.HasValue)
                query = query.Where(x => x.LinkedEntityType == (int)request.ImageUploadType.Value);

            var result = await query.ToListAsync(cancellationToken);

            var response = result.Select(item => new GetPictureListResponse
            {
                Id = item.Id,
                Name = item.Name,
                AltText = item.AltText,
                Size = item.Size,
                Url = item.GetFileUrl(_baseUrl),
                ImageUploadType = item.LinkedEntityType,
                ImageUploadTypeName = GetUploadTypeName(item.LinkedEntityType),
            }).ToList();

            return response;
        }

        private string GetUploadTypeName(int? uploadType)
        {
            if (uploadType == null)
                return null;
            var type = EnumHelper.GetEnumValue(uploadType.Value);
            return type != null ? EnumHelper.GetEnumDescription(type.Value) : null;
        }
    }
}
