using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.PurgeGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ResourceTracker.Application.Features.Commands.GameCommands
{
    public class GameCommandsHandler :
        ICommandHandler<CreateGameWithImageCommand, CreateGameWithImageResponse>,
        ICommandHandler<CreateGameCommand, CreateGameResponse>,
        ICommandHandler<UpdateGameCommand>,
        ICommandHandler<DeleteGameCommand>,
        ICommandHandler<PurgeGameCommand, PurgeGameResponse>
    {

        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;
        private readonly IImageService _imageStorageService;
        public GameCommandsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo, IImageService imageStorageService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
            _imageStorageService = imageStorageService;
        }

        public async Task<CreateGameWithImageResponse> Handle(CreateGameWithImageCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            Picture picture = new Picture();
            if (command.Image != null)
            {
                picture = await _imageStorageService.UploadImage(command.Image, (int)ImageUploadTypeEnum.Game, nameof(Game), _userInfo.GetUserId(), command.AltText ?? string.Empty, cancellationToken);
                
            }
            Game item = new Game
            {
                Name = command.Name,
                Description = command.Description ?? string.Empty,
                PictureId = picture.Id != 0 ? picture.Id : null
            };
            await _repo.InsertAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);


            return new CreateGameWithImageResponse(item.Id);
        }
        
        public async Task<Unit> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            var item = await _repo.Games.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException("Invalid Game");

            item.Name = command.Name;
            item.Description = command.Description;

            await _repo.UpdateAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            var item = await _repo.Games.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException("Invalid Game");

            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }

        public async Task<PurgeGameResponse> Handle(PurgeGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            var game = await _repo.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (game == null)
                throw new NotFoundException(nameof(Game), command.Id);

            // Destructive and irreversible, so the caller names the game it is wiping: a wrong id
            // cannot quietly delete a different game.
            if (!string.Equals(game.Name, command.ConfirmName, StringComparison.Ordinal))
            {
                throw new BadRequestException($"ConfirmName does not match game {game.Id}. Send its exact name to purge it.");
            }

            // Pictures are referenced from Game/Component/Quest.PictureId - there is no foreign key
            // back from Picture - so collect them before the rows that point at them are gone.
            var pictures = await _repo.Set<Picture>()
                .Where(p =>
                    p.Games.Any(g => g.Id == command.Id) ||
                    p.Components.Any(c => c.GameId == command.Id) ||
                    p.Quests.Any(q => q.GameSave.GameId == command.Id))
                .Select(p => new { p.Id, p.Path })
                .ToListAsync(cancellationToken);

            // Children first, so the RESTRICT foreign keys (Component on Game, Component on
            // RecipeComponent, etc.) are never violated. The whole set runs in one transaction:
            // either the game is gone with all of it, or nothing was touched.
            var response = await _unitOfWork.ExecuteInTransaction(async ct =>
            {
                var result = new PurgeGameResponse { GameId = game.Id, GameName = game.Name };

                result.RecipeComponentsDeleted = await _repo.Set<RecipeComponent>()
                    .Where(rc => rc.Recipe.Component.GameId == command.Id).ExecuteDeleteAsync(ct);

                result.RecipesDeleted = await _repo.Set<Recipe>()
                    .Where(r => r.Component.GameId == command.Id).ExecuteDeleteAsync(ct);

                result.InventoryRowsDeleted = await _repo.Set<QuestComponents>()
                    .Where(qc => qc.Component.GameId == command.Id || qc.Quest.GameSave.GameId == command.Id)
                    .ExecuteDeleteAsync(ct);

                result.PlayerFacilitiesDeleted = await _repo.Set<PlayerFacility>()
                    .Where(pf => pf.GameSave.GameId == command.Id || pf.Component.GameId == command.Id)
                    .ExecuteDeleteAsync(ct);

                result.BuildPlanComponentsDeleted = await _repo.Set<BuildPlanComponent>()
                    .Where(bpc => bpc.Component.GameId == command.Id || bpc.BuildPlan.GameSave.GameId == command.Id)
                    .ExecuteDeleteAsync(ct);

                result.BuildPlanQuestsDeleted = await _repo.Set<BuildPlanQuest>()
                    .Where(bpq => bpq.Quest.GameSave.GameId == command.Id || bpq.BuildPlan.GameSave.GameId == command.Id)
                    .ExecuteDeleteAsync(ct);

                result.BuildPlansDeleted = await _repo.Set<BuildPlan>()
                    .Where(bp => bp.GameSave.GameId == command.Id).ExecuteDeleteAsync(ct);

                result.QuestsDeleted = await _repo.Set<Quest>()
                    .Where(q => q.GameSave.GameId == command.Id).ExecuteDeleteAsync(ct);

                result.GameSavesDeleted = await _repo.Set<GameSave>()
                    .Where(gs => gs.GameId == command.Id).ExecuteDeleteAsync(ct);

                result.ComponentsDeleted = await _repo.Set<Component>()
                    .Where(c => c.GameId == command.Id).ExecuteDeleteAsync(ct);

                if (pictures.Count > 0)
                {
                    var pictureIds = pictures.Select(p => p.Id).ToList();
                    result.PicturesDeleted = await _repo.Set<Picture>()
                        .Where(p => pictureIds.Contains(p.Id)).ExecuteDeleteAsync(ct);
                }

                await _repo.Set<Game>()
                    .Where(g => g.Id == command.Id).ExecuteDeleteAsync(ct);

                return result;
            }, cancellationToken);

            // Files after the database has committed. If this fails the data is already consistent
            // and the leftovers are unreferenced, rather than rows pointing at deleted images.
            response.ImageFilesDeleted = await _imageStorageService
                .DeleteImageFiles(pictures.Select(p => (string?)p.Path), cancellationToken);

            return response;
        }

        public async Task<CreateGameResponse> Handle(CreateGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");


            Game item = new Game
            {
                Name = command.Name,
                Description = command.Description ?? string.Empty,
                PictureId = null
            };
            await _repo.InsertAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);


            return new CreateGameResponse(item.Id);
        }
    }
}
