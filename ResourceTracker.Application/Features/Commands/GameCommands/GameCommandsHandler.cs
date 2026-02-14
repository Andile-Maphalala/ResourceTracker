using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.GameCommands
{
    public class GameCommandsHandler :
        ICommandHandler<CreateGameCommand, CreateGameResponse>,
        ICommandHandler<UpdateGameCommand>,
        ICommandHandler<DeleteGameCommand>
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

        public async Task<CreateGameResponse> Handle(CreateGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            Picture picture = new Picture();
            if (command.Image != null)
            {
                picture = await _imageStorageService.UploadImage(command.Image, nameof(Game), _userInfo.GetUserId(), command.AltText, cancellationToken);
                
            }
            Game item = new Game
            {
                Name = command.Name,
                Description = command.Description,
                PictureId = picture.Id != 0 ? picture.Id : null
            };
            await _repo.InsertAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);


            return new CreateGameResponse(item.Id);
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
    }
}
