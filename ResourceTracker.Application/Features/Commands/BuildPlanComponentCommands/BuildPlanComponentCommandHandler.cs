using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponents;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponents;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponents;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands
{
    public class BuildPlanComponentCommandHandler :
        ICommandHandler<CreateBuildPlanComponentCommand, CreateBuildPlanComponentResponse>,
        ICommandHandler<CreateBuildPlanComponentsCommand, CreateBuildPlanComponentsResponse>,
        ICommandHandler<UpdateBuildPlanComponentCommand>,
        ICommandHandler<UpdateBuildPlanComponentsCommand>,
        ICommandHandler<DeleteBuildPlanComponentCommand>,
        ICommandHandler<DeleteBuildPlanComponentsCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public BuildPlanComponentCommandHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateBuildPlanComponentResponse> Handle(CreateBuildPlanComponentCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var entity = new BuildPlanComponent
            {
                Order = command.Order,
                QuantityNeeded = command.QuantityNeeded,
                ComponentId = command.ComponentId,
                BuildPlanId = command.BuildPlanId
            };

            await _repo.InsertAsync(entity, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateBuildPlanComponentResponse(entity.Id);
        }

        public async Task<CreateBuildPlanComponentsResponse> Handle(CreateBuildPlanComponentsCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var items = new List<BuildPlanComponent>();

            foreach (var c in command.Commands)
            {
                items.Add(new BuildPlanComponent
                {
                    Order = c.Order,
                    QuantityNeeded = c.QuantityNeeded,
                    ComponentId = c.ComponentId,
                    BuildPlanId = command.BuildPlanId
                });
            }

            await _repo.BulkInsertAsync(items, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateBuildPlanComponentsResponse(items.Count);
        }

        public async Task<Unit> Handle(UpdateBuildPlanComponentCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var item = await _repo.BuildPlanComponents.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(BuildPlanComponent), command.Id);

            item.Order = command.Order;
            item.QuantityNeeded = command.QuantityNeeded;

            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateBuildPlanComponentsCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var updatedItems = new List<BuildPlanComponent>();

            foreach (var cmd in command.Commands)
            {
                var item = await _repo.BuildPlanComponents.FirstOrDefaultAsync(x => x.Id == cmd.Id, cancellationToken);
                if (item == null)
                    throw new NotFoundException(nameof(BuildPlanComponent), cmd.Id);

                var isUpdated = false;

                if (item.Order != cmd.Order)
                {
                    item.Order = cmd.Order;
                    isUpdated = true;
                }

                if (item.QuantityNeeded != cmd.QuantityNeeded)
                {
                    item.QuantityNeeded = cmd.QuantityNeeded;
                    isUpdated = true;
                }

                if (isUpdated)
                    updatedItems.Add(item);
            }

            if (updatedItems.Any())
                await _repo.BulkUpdateAsync(updatedItems, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteBuildPlanComponentCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.BuildPlanComponents.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(BuildPlanComponent), command.Id);

            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteBuildPlanComponentsCommand request, CancellationToken cancellationToken)
        {
            foreach (var id in request.Ids)
            {
                var item = await _repo.BuildPlanComponents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item == null)
                    throw new NotFoundException(nameof(BuildPlanComponent), id);

                await _repo.DeleteAsync(item, cancellationToken);
            }

            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
