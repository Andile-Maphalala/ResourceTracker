
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.PictureCommands.CreatePicture;
using ResourceTracker.Application.Features.Commands.PictureCommands.DeletePicture;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Commands.PictureCommands
{
    public class PictureCommandsHandler : ICommandHandler<CreatePictureCommand, CreatePictureResponse>,
                                          ICommandHandler<DeletePictureCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IResourceTrackerRepository _repo;
        private readonly IUserInfo _userInfo;
        private readonly IImageService _imageService;
        private readonly string _baseUrl;
        public PictureCommandsHandler(IUnitOfWork unitOfWork, IResourceTrackerRepository repo, IImageService imageService, IUserInfo userInfo, IOptions<ApplicationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _imageService = imageService;
            _userInfo = userInfo;
            _baseUrl = options.Value.BaseUrl ?? throw new InvalidOperationException("BaseUrl is missing.");
        }

        public async Task<CreatePictureResponse> Handle(CreatePictureCommand request, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            if (request.ImageUploadType == ImageUploadTypeEnum.Game && _userInfo.IsAdmin() == false)
                throw new BadRequestException("Unauthorised action");

            var picture = await _imageService.UploadImage(request.Data, (int)request.ImageUploadType, request.ImageUploadType.ToString(), userId, request.AltText ?? string.Empty, cancellationToken);

            switch(request.ImageUploadType)
            {
                case ImageUploadTypeEnum.Game:
                    await UpdateGamePicture(request.LinkedEntityId, picture.Id, cancellationToken);
                    break;
                case ImageUploadTypeEnum.Quest:
                    await UpdateQuestPicture(request.LinkedEntityId, picture.Id, cancellationToken);
                    break;
                case ImageUploadTypeEnum.Component:
                    await UpdateComponentPicture(request.LinkedEntityId, picture.Id, cancellationToken);
                    break;
                default:
                    throw new BadRequestException("Invalid ImageUploadType");
            }

            await _unitOfWork.Save(cancellationToken);
            var response = new CreatePictureResponse
            {
                Id = picture.Id,
                Url = picture.GetFileUrl(_baseUrl)
            };

            return response;
        }


        public async Task<Unit> Handle(DeletePictureCommand request, CancellationToken cancellationToken)
        {
            var userId = _userInfo.GetUserId();
            if (userId == 0)
                throw new BadRequestException("Invalid User");

            await _imageService.DeleteImage(request.Id, cancellationToken);
            return Unit.Value;
        }

        private async Task UpdateGamePicture(int id, int pictureId, CancellationToken cancellationToken)
        {
            var item = await _repo.Games.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (item == null)
                throw new NotFoundException(nameof(Game), id);

            item.PictureId = pictureId;
        }

        private async Task UpdateQuestPicture(int id, int pictureId, CancellationToken cancellationToken)
        {
            var item = await _repo.Quests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (item == null)
                throw new NotFoundException(nameof(Quest), id);

            item.PictureId = pictureId;
        }

        private async Task UpdateComponentPicture(int id, int pictureId, CancellationToken cancellationToken)
        {
            var item = await _repo.Components.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (item == null)
                throw new NotFoundException(nameof(Component), id);

            item.PictureId = pictureId;
        }

    }
}
