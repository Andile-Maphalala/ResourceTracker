
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using ResourceTracker.Application.Features.Commands.ImportCommands.ImportGame;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Application.Services;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.ImportCommands
{
    public class ImportCommandsHandler : ICommandHandler<ImportGameCommand, ImportGameResponse>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;
        private readonly IImageService _imageStorageService;

        public ImportCommandsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo, IImageService imageStorageService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
            _imageStorageService = imageStorageService;
        }

        public async Task<ImportGameResponse> Handle(ImportGameCommand command, CancellationToken cancellationToken)
        {
            var isAdmin = _userInfo.IsAdmin();
            if (!isAdmin)
            {
                throw new BadRequestException("Unauthorised action");

            }
            if (command.File == null || command.File.Length == 0)
            {
                throw new BadRequestException("File is empty");
            }

            var gameData =  await command.File.ToObjectAsync<GameDataDto>();
            var components = ImportService.ImportComponents(gameData, command.GameId);
            foreach (var component in components)
            {
                var dto = gameData.Components.FirstOrDefault(c => c.Name == component.Name);
                if (dto != null && dto.Image_url != null)
                {
                    var picture = await _imageStorageService.UploadImage(dto.Image_url, (int)ImageUploadTypeEnum.Component, nameof(Component), _userInfo.GetUserId(), dto.Name, cancellationToken);
                    component.PictureId = picture.Id;
                }
            }
            await _repo.BulkInsertAndUpdateIdsAsync(components, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            var recipes = ImportService.ImportRecipes(gameData, command.GameId, components, cancellationToken);
            await _repo.BulkInsertAsync(recipes, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
            return new ImportGameResponse();
        }
    }
}
