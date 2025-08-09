using MediatR;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.QueryBuilders;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Queries.GameQueries
{
    public class GameQueriesHandler:
        IRequestHandler<GetGameQuery, GetGameResponse>,
        IRequestHandler<SearchGamesQuery, PageableResponse<SearchGamesResponse>>
    {
        private readonly IResourceTrackerRepository _repo;

        public GameQueriesHandler(IResourceTrackerRepository repo)
        {
            _repo = repo;
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
                }).FirstOrDefaultAsync(cancellationToken);
            if (game is null)
                throw new NotFoundException(nameof(Game), request.Id);
            return game;
        }

        public async Task<PageableResponse<SearchGamesResponse>> Handle(SearchGamesQuery request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.OrderBy))
                request.OrderBy = nameof(SearchGamesResponse.Id);

            var result = await _repo.Games
                .ApplyFilters(request)
                .Select(x => new SearchGamesResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                }).ToPageableListAsync(request, cancellationToken);

            return result;
        }
    }
}
