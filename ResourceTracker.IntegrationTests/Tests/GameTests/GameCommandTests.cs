using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.GameCommands;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;
using System.Text;

namespace ResourceTracker.IntegrationTests.Tests.GameTests
{

    public class GameCommandTests : IntegrationTestBase
    {
        private GameCommandsHandler _handler;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _handler = new GameCommandsHandler(_serviceScope.ServiceProvider.GetRequiredService<IResourceTrackerRepository>(),
                _serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>(),
                UserInfoMock.Object,
                _serviceScope.ServiceProvider.GetRequiredService<IImageService>());

        }

        [Fact]
        public async Task CreateGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(false);

            var command = new CreateGameCommand
            {
                Name = "Test",
                Description = "Desc"
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateGameCommand_AdminNoImage_CreateRecord()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(true);

            var command = new CreateGameCommand
            {
                Name = "Test Game",
                Description = "This is a test game"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var game = await _dbContext.Games.FindAsync(result.Id);
            game.Should().NotBeNull();
            game.PictureId.Should().BeNull();
            game.Name.Should().Be(command.Name);
            game.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task CreateGameCommand_AdminWithImage_CreateRecord()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(true);
            UserInfoMock
            .Setup(x => x.GetUserId())
            .Returns(1);

            var command = new CreateGameCommand
            {
                Name = "Test Game with Image",
                Description = "This is a test game with image",
                Image = CreateTestImage(),
                AltText = "Test Image Alt Text"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var game = await _dbContext.Games.FindAsync(result.Id);
            game.Should().NotBeNull();
            game.PictureId.Should().NotBeNull();
            game.Name.Should().Be(command.Name);
            game.Description.Should().Be(command.Description);

            var picture = await _dbContext.Pictures.FindAsync(game.PictureId);
            picture.Should().NotBeNull();
            picture.AltText.Should().Be(command.AltText);
            picture.UploadedBy.Should().Be(1);

        }

        [Fact]
        public async Task UpdateGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(false);

            var command = new UpdateGameCommand
            {
                Name = "Test",
                Description = "Desc"
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateGameCommand_Admin_CreateRecord()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(true);

            var gameId = await AddRecord();
            var command = new UpdateGameCommand
            {
                Id = gameId,
                Name = "Update Game",
                Description = "This is a test game"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            var game = await _dbContext.Games.FindAsync(gameId);
            game.Should().NotBeNull();
            game.PictureId.Should().BeNull();
            game.Name.Should().Be(command.Name);
            game.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task DeleteGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(false);

            var command = new DeleteGameCommand
            {
                Id = 1
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteGameCommand_AdminNoImage_CreateRecord()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.IsAdmin())
            .Returns(true);

            var gameId = await AddRecord();

            var command = new DeleteGameCommand
            {
                Id = gameId
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            var game = await _dbContext.Games.FindAsync(gameId);
            game.Should().BeNull();
        }

        private IFormFile CreateTestImage(string content = "test image")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", "test.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        private async Task<int> AddRecord()
        {
            var game = new Game
            {
                Name = "Existing Game",
                Description = "Existing Description",
            };
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();
            return game.Id;
        }

    }
}
