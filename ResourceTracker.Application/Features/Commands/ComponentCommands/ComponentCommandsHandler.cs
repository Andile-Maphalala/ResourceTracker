using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent;
using ResourceTracker.Application.Interfaces;
using Component = ResourceTracker.Domain.Entities.Component;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands
{
    public class ComponentCommandsHandler:
        ICommandHandler<CreateComponentCommand, CreateComponentResponse>,
        ICommandHandler<UpdateComponentCommand>,
        ICommandHandler<DeleteComponentCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public ComponentCommandsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo = null)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateComponentResponse> Handle(CreateComponentCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            Component item = new Component
            {
                Name = command.Name,
                Description = command.Description,
                Type = command.Type,
                GameId = command.GameId
            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateComponentResponse(item.Id);
        }

        public async Task<Unit> Handle(UpdateComponentCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(Component),command.Id);
            }

            item.Name = command.Name;
            item.Description = command.Description;
            item.Type = command.Type;

            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }


        public async Task<Unit> Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
                throw new BadRequestException("Unauthorised action");

            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(Component), command.Id);
            }

            await _repo.DeleteAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
