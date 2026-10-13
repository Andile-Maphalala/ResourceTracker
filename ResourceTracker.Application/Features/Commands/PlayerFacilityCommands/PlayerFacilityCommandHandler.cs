using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.CreatePlayerFacility;
using ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.DeletePlayerFacility;
using ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.UpdatePlayerFacility;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands
{
    public class PlayerFacilityCommandHandler :
        ICommandHandler<CreatePlayerFacilityCommand, CreatePlayerFacilityResponse>,
        ICommandHandler<UpdatePlayerFacilityCommand>,
        ICommandHandler<DeletePlayerFacilityCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public PlayerFacilityCommandHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreatePlayerFacilityResponse> Handle(CreatePlayerFacilityCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var quest = await _repo.Quests.FirstOrDefaultAsync(x => x.Id == command.QuestId, cancellationToken);
            if (quest == null)
            {
                throw new NotFoundException(nameof(Quest), command.QuestId);
            }

            var component = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.ComponentId, cancellationToken);
            if (component == null)
            {
                throw new NotFoundException(nameof(Component), command.ComponentId);
            }

            // A PlayerFacility says "a facility of this kind stands here". Letting a resource or
            // composite in would hand the resolver something it treats as capability but that the
            // consume path treats as stock, which is the split this table exists to remove.
            if (component.Type != (int)ComponentTypeEnum.Facility)
            {
                throw new BadRequestException($"{component.Name} is not a facility and cannot be recorded against a location");
            }

            PlayerFacility item = new PlayerFacility
            {
                ComponentId = component.Id,
                QuestId = quest.Id,
                GameSaveId = quest.GameSaveId,
                Level = command.Level
            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreatePlayerFacilityResponse(item.Id);
        }

        public async Task<Unit> Handle(UpdatePlayerFacilityCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var item = await _repo.PlayerFacilities.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(PlayerFacility), command.Id);
            }

            item.Level = command.Level;

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeletePlayerFacilityCommand command, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            var item = await _repo.PlayerFacilities.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(PlayerFacility), command.Id);
            }

            await _repo.DeleteAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
