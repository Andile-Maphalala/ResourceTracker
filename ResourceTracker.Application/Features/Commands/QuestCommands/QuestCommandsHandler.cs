using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest;
using ResourceTracker.Application.Repositories;
using Component = ResourceTracker.Domain.Entities.Component;

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

        public QuestCommandsHandler(IResourceTrackerRepository repo, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateQuestResponse> Handle(CreateQuestCommand command, CancellationToken cancellationToken)
        {
            //var item = _mapper.Map<Component>(command);
            Component item = new Component
            {
                Name = command.Name,
                Description = command.Description,
            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateQuestResponse(item.Id);
        }

        public async Task Handle(UpdateQuestCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid component");
            }

            //item = _mapper.Map(command, item);
            item.Name = command.Name;
            item.Description = command.Description;

            await _unitOfWork.Save(cancellationToken);
        }


        public async Task Handle(DeleteQuestCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid quest");
            }

            await _repo.DeleteAsync<Component>(x => x.Id == item.Id, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
        }
    }
}
