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
    public class QuestCommandsHandler:
        ICommandHandler<CreateQuestCommand, CreateQuestResponse>,
        IRequestHandler<UpdateQuestCommand>,
        IRequestHandler<DeleteQuestCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;


        public QuestCommandsHandler(IResourceTrackerRepository repo, IMapper mapper, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _mapper = mapper;
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
                UserId = userId,

            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateQuestResponse(item.Id);
        }

        public async Task Handle(UpdateQuestCommand command, CancellationToken cancellationToken)
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

            await _unitOfWork.Save(cancellationToken);
        }


        public async Task Handle(DeleteQuestCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Quests.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid quest");
            }

            await _repo.DeleteAsync<Quest>(x => x.Id == item.Id, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
        }
    }
}
