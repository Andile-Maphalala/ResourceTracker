using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent;
using ResourceTracker.Application.Repositories;
using Component = ResourceTracker.Domain.Entities.Component;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands
{
    public class ComponentCommandsHandler:
        ICommandHandler<CreateComponentCommand, CreateComponentResponse>,
        IRequestHandler<UpdateComponentCommand>,
        IRequestHandler<DeleteComponentCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ComponentCommandsHandler(IResourceTrackerRepository repo, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateComponentResponse> Handle(CreateComponentCommand command, CancellationToken cancellationToken)
        {
            //var item = _mapper.Map<Component>(command);
            Component item = new Component
            {
                Name = command.Name,
                Description = command.Description,
                Type = command.Type
            };

            await _repo.InsertAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);

            return new CreateComponentResponse(item.Id);
        }

        public async Task Handle(UpdateComponentCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid component");
            }

            //item = _mapper.Map(command, item);
            item.Name = command.Name;
            item.Description = command.Description;
            item.Type = command.Type;

            await _unitOfWork.Save(cancellationToken);
        }


        public async Task Handle(DeleteComponentCommand command, CancellationToken cancellationToken)
        {
            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
            {
                throw new BadRequestException("Invalid component");
            }

            await _repo.DeleteAsync(item, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
        }
    }
}
