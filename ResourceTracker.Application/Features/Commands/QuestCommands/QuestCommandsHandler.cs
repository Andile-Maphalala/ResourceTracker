using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest;
using ResourceTracker.Application.Repositories;
using Quest = ResourceTracker.Domain.Entities.Quest;
using ResourceTracker.Application.Common.User;

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
            var userId = _userInfo.GetUserId();
            if(userId == 0)
                throw new BadRequestException("Invalid User");

            Quest item = new Quest
            {
                Name = command.Name,
                Description = command.Description,
                Location = command.Location,
                UserId = userId,

            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateQuestResponse(item.Id);
        }

        public async Task<Unit> Handle(UpdateQuestCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var item = await _repo.Quests.FirstOrDefaultAsync(x => x.Id == command.Id && x.UserId == userId, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid Quest");
            }

            item.Name = command.Name;
            item.Description = command.Description;
            item.Location = command.Location;

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

            await _repo.DeleteAsync<Quest>(x => x.Id == item.Id, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
