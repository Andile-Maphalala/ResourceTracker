using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList;
using ResourceTracker.Application.Interfaces;

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries
{
    public class GameSaveQueriesHandler :
        IRequestHandler<GetGameSaveQuery, GetGameSaveResponse>,
        IRequestHandler<GetGameSaveListQuery, List<GetGameSaveListResponse>>
    {
        private readonly IResourceTrackerRepository _repo;

        public GameSaveQueriesHandler(IResourceTrackerRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetGameSaveResponse> Handle(GetGameSaveQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.GameSaves
                .Where(x => x.Id == request.Id)
                .Select(x => new GetGameSaveResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Created = x.Created,
                    GameId = x.Game.Id,
                    GameName = x.Game.Name
                }).FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                throw new NotFoundException("Game Save", request.Id);

            return response;
        }

        public async Task<List<GetGameSaveListResponse>> Handle(GetGameSaveListQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.GameSaves
                .Where(x => x.GameId == request.GameId)
                .Select(x => new GetGameSaveListResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Created = x.Created
                }).ToListAsync(cancellationToken);

            return response;
        }
    }
}
