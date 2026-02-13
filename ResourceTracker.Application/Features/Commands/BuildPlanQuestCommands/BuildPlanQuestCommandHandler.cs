using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.CreateBuildPlanQuest;
using ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.DeleteBuildPlanQuest;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands
{
    public class BuildPlanQuestCommandHandler :
        ICommandHandler<CreateBuildPlanQuestCommand, CreateBuildPlanQuestResponse>,
        ICommandHandler<DeleteBuildPlanQuestCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public BuildPlanQuestCommandHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateBuildPlanQuestResponse> Handle(CreateBuildPlanQuestCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var entity = new BuildPlanQuest
            {
                BuildPlanId = command.BuildPlanId,
                QuestId = command.QuestId
            };

            await _repo.InsertAsync(entity, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateBuildPlanQuestResponse(entity.BuildPlanId);
        }

        public async Task<Unit> Handle(DeleteBuildPlanQuestCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.BuildPlanQuests.FirstOrDefaultAsync(x => x.BuildPlanId == command.BuildPlanId && x.QuestId == command.QuestId, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(BuildPlanQuest), $"BuildPlanId:{command.BuildPlanId}, QuestId:{command.QuestId}");

            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
