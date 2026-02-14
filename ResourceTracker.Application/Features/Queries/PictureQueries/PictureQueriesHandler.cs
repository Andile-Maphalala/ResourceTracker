

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.PictureQueries.GetPicture;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Queries.PictureQueries
{
    public class PictureQueriesHandler : IRequestHandler<GetPictureQuery, GetPictureResponse>
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
                Url = item.GetFileUrl(_baseUrl)
            };
            return response;

        }
    }
}
