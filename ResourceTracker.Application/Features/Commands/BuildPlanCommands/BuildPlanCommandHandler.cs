using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.CreateBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.UpdateBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.DeleteBuildPlan;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.BuildPlanCommands
{
    public class BuildPlanCommandHandler :
        ICommandHandler<CreateBuildPlanCommand, CreateBuildPlanResponse>,
        ICommandHandler<UpdateBuildPlanCommand>,
        ICommandHandler<DeleteBuildPlanCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public BuildPlanCommandHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateBuildPlanResponse> Handle(CreateBuildPlanCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var entity = new BuildPlan
            {
                Name = command.Name,
                Description = command.Description,
                GameSaveId = command.GameSaveId,
                IncludeInventory = command.IncludeInventory,
                IncludeFacilityRequirements = command.IncludeFacilityRequirements,
            };

            await _repo.InsertAsync(entity, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateBuildPlanResponse(entity.Id);
        }

        public async Task<Unit> Handle(UpdateBuildPlanCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var item = await _repo.BuildPlans.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(BuildPlan), command.Id);

            item.Name = command.Name;
            item.Description = command.Description;
            item.IncludeFacilityRequirements = command.IncludeFacilityRequirements;
            item.IncludeInventory = command.IncludeInventory;

            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteBuildPlanCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.BuildPlans.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(BuildPlan), command.Id);

            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
