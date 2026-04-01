using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.ComponentQueries
{
    public class ComponentQueriesHandler:
        IRequestHandler<GetComponentQuery,GetComponentResponse>,
        IRequestHandler<SearchComponentsQuery, PageableResponse<SearchComponentsResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public ComponentQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;

        }

        public async Task<GetComponentResponse> Handle(GetComponentQuery request, CancellationToken cancellationToken)
        {
            var quest = await _repo.Components
                 .Where(x => x.Id == request.Id)
                 .Select(x => new GetComponentResponse
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     Type = x.Type,
                     TypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)x.Type),
                     ImageUrl = x.Picture.GetFileUrl(_baseUrl),
                     GameId = x.GameId,
                     GameName = x.Game.Name
                 }).FirstOrDefaultAsync(cancellationToken);

            if (quest is null)
                throw new NotFoundException(nameof(Component), request.Id);

            return quest;
        }

        public async Task<PageableResponse<SearchComponentsResponse>> Handle(SearchComponentsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.OrderBy))
                request.OrderBy = nameof(Component.Id);

            var quests = await _repo.Search(request)
                 .Select(x => new SearchComponentsResponse
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     Type = x.Type,
                     TypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)x.Type),
                     ImageUrl = x.Picture.GetFileUrl(_baseUrl)
                 }).ToPageableListAsync(request, cancellationToken);

            return quests;
        }
    }
}
