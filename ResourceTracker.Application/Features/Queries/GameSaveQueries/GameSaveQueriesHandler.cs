using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries
{
    public class GameSaveQueriesHandler :
        IRequestHandler<GetGameSaveQuery, GetGameSaveResponse>,
        IRequestHandler<GetGameSaveListQuery, List<GetGameSaveListResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUserInfo _userInfo;
        private readonly string _baseUrl;

        public GameSaveQueriesHandler(IResourceTrackerRepository repo, IUserInfo userInfo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _userInfo = userInfo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetGameSaveResponse> Handle(GetGameSaveQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.GameSaves
                .Where(x => x.Id == request.Id && x.UserId == _userInfo.GetUserId())
                .Select(x => new GetGameSaveResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Created = x.Created,
                    GameId = x.Game.Id,
                    GameName = x.Game.Name,
                    GameImageUrl = x.Game.Picture.GetFileUrl(_baseUrl)
                }).FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                throw new NotFoundException("Game Save", request.Id);

            return response;
        }

        public async Task<List<GetGameSaveListResponse>> Handle(GetGameSaveListQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.GameSaves
                .Where(x => x.GameId == request.GameId && x.UserId == _userInfo.GetUserId())
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
