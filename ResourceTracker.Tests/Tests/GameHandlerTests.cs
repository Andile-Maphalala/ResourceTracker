using Moq;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.GameCommands;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Tests.Tests
{
    public class GameHandlerTests
    {
        private readonly Mock<IResourceTrackerRepository> _repo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IUserInfo> _userInfo;
        private readonly Mock<IImageService> _imageStorageService;

        public GameHandlerTests()
        {
            _repo = new();
            _unitOfWork = new();
            _userInfo = new();
            _imageStorageService = new();
        }


        [Fact]
        public async Task Handle_should_return_success()
        {
            // Arrange
            _userInfo.Setup(x => x.IsAdmin()).Returns(true);
            var command = new CreateGameCommand
            {
                Name = "Test Game",
                Description = "This is a test game",
                Image = null,
                AltText = "Test Image"
            };
            var handler = new GameCommandsHandler(_repo.Object, _unitOfWork.Object, _userInfo.Object, _imageStorageService.Object);
            // Act
            var result = await handler.Handle(command, default);
            // Assert
            _repo.Verify(x => x.InsertAsync(It.Is<Game>(g => g.Id == result.Id), default),
                Times.Once);
        }
    }
}
