using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Queries.GameQueries
{
    public class GameQueriesHandler:
        IRequestHandler<GetGameQuery, GetGameResponse>,
        IRequestHandler<SearchGamesQuery, PageableResponse<SearchGamesResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public GameQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetGameResponse> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = await _repo.Games
                .Where(x => x.Id == request.Id)
                .Select(x => new GetGameResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.Picture.GetFileUrl(_baseUrl), 
                    PictureId = x.PictureId
                }).FirstOrDefaultAsync(cancellationToken);

            if (game is null)
                throw new NotFoundException(nameof(Game), request.Id);

            return game;
        }

        public async Task<PageableResponse<SearchGamesResponse>> Handle(SearchGamesQuery request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.OrderBy))
                request.OrderBy = nameof(SearchGamesResponse.Id);

            var result = await _repo.Search(request)
                .Select(x => new SearchGamesResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.Picture.GetFileUrl(_baseUrl),
                    PictureId = x.PictureId
                }).ToPageableListAsync(request, cancellationToken);

            return result;
        }
    }
}
