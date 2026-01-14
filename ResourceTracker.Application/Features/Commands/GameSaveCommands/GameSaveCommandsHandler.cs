
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.CreateGameSave;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.DeleteGameSave;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.UpdateGameSave;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands
{
    public class GameSaveCommandsHandler :
        ICommandHandler<CreateGameSaveCommand, CreateGameSaveResponse>,
        ICommandHandler<UpdateGameSaveCommand>,
        ICommandHandler<DeleteGameSaveCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public GameSaveCommandsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateGameSaveResponse> Handle(CreateGameSaveCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            GameSave item = new GameSave
            {
                Name = command.Name,
                Description = command.Description,
                GameId = command.GameId,
                UserId = userId,
                Created = DateTime.UtcNow
            };

            await _repo.InsertAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateGameSaveResponse(item.Id);
        }

        public async Task<Unit> Handle(UpdateGameSaveCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.GameSaves.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException("Invalid GameSave");
            }

            var userId = _userInfo.GetUserId();
            if(item.UserId != userId)
            { 
               throw new BadRequestException("You do not have access to this content");
            }

            item.Name = command.Name;
            item.Description = command.Description;

            await _repo.UpdateAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteGameSaveCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.GameSaves.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException("Invalid GameSave");
            }

            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
