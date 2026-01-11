using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.QuestCommands
{
    public class QuestCommandsHandler :
        ICommandHandler<CreateQuestCommand, CreateQuestResponse>,
        ICommandHandler<UpdateQuestCommand>,
        ICommandHandler<DeleteQuestCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;


        public QuestCommandsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateQuestResponse> Handle(CreateQuestCommand command, CancellationToken cancellationToken)
        {
            Quest item = new Quest
            {
                Name = command.Name,
                Description = command.Description,
                Location = command.Location,
                GameSaveId = command.GameSaveId
            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateQuestResponse(item.Id);
        }

        public async Task<Unit> Handle(UpdateQuestCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Quests.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid Quest");
            }

            item.Name = command.Name;
            item.Description = command.Description;
            item.Location = command.Location;
            item.GameSaveId = command.GameSaveId;

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }


        public async Task<Unit> Handle(DeleteQuestCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Quests.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid quest");
            }

            await _repo.DeleteAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
