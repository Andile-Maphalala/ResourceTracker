using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public GameCommandsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateGameResponse> Handle(CreateGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            Game item = new Game
            {
                Name = command.Name,
                Description = command.Description,
                CoverImage = new byte[0],   
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
                throw new NotFoundException("Game not found");

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
                throw new NotFoundException("Game not found");

            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
