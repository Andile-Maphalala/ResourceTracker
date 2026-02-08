using MediatR;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponents;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponents;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands
{
    public class QuestComponentCommandHandler : 
        ICommandHandler<CreateQuestComponentCommand,CreateQuestComponentResponse>,
        ICommandHandler<UpdateQuestComponentCommand>,
        ICommandHandler<DeleteQuestComponentCommand>,
        ICommandHandler<CreateQuestComponentsCommand, CreateQuestComponentsResponse>,
        ICommandHandler<UpdateQuestComponentsCommand>,
        ICommandHandler<DeleteQuestComponentsCommand>

    {

        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public QuestComponentCommandHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }


        public async Task<CreateQuestComponentResponse> Handle(CreateQuestComponentCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            QuestComponents item = new QuestComponents
            {
                AmountAquired = command.AmountAquired,
                ComponentId = command.ComponentId,
                QuestId = command.QuestId,

            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateQuestComponentResponse(item.Id);
        }

        public async Task<CreateQuestComponentsResponse> Handle(CreateQuestComponentsCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            List<QuestComponents> items = new List<QuestComponents>();

            foreach (CreateQuestComponentClass questComponent in command.Commands)
            {
                QuestComponents item = new QuestComponents
                {
                    AmountAquired = questComponent.AmountAquired,
                    ComponentId = questComponent.ComponentId,
                    QuestId = command.QuestId,
                };

                items.Add(item);
            }
            await _repo.BulkInsertAsync(items, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateQuestComponentsResponse(items.Count());

        }

        public async Task<Unit> Handle(UpdateQuestComponentCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var item = await _repo.QuestComponents.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(QuestComponents),command.Id);
            }

            item.AmountAquired = command.AmountAquired;

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateQuestComponentsCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            List<QuestComponents> updatedItems = new List<QuestComponents>();

            foreach (UpdateQuestComponentCommand questComponent in command.Commands)
            {
                var item = await _repo.QuestComponents.FirstOrDefaultAsync(x => x.Id == questComponent.Id, cancellationToken);
                if (item == null)
                {
                    throw new NotFoundException(nameof(QuestComponents), questComponent.Id);
                }

                bool isUpdated = false;

                if (item.AmountAquired != questComponent.AmountAquired)
                {
                    item.AmountAquired = questComponent.AmountAquired;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    updatedItems.Add(item);
                }
            }

            if (updatedItems.Any())
            {
                await _repo.BulkUpdateAsync(updatedItems, cancellationToken);
            }

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;

        }

        public async Task<Unit> Handle(DeleteQuestComponentCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.QuestComponents.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(QuestComponents), command.Id); ;
            }

            await _repo.DeleteAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;

        }

        public async Task<Unit> Handle(DeleteQuestComponentsCommand request, CancellationToken cancellationToken)
        {
            foreach (int id in request.Ids)
            {
                var item = await _repo.QuestComponents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (item == null)
                {
                    throw new NotFoundException(nameof(QuestComponents), id); ;
                }
                await _repo.DeleteAsync(item, cancellationToken);
            }
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
