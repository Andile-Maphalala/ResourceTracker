using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;
using System.Text;

namespace ResourceTracker.IntegrationTests.Tests.GameTests
{

    public class GameCommandTests : IntegrationTestBase
    {
        public GameCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CreateGame_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            var command = new CreateGameCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task CreateGame_NameEmpty_ThrowError()
        {
            // Arrange
            var command = new CreateGameCommand
            {
                Name = "",
                Description = "Desc",
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateGame_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            var command = new CreateGameCommand
            {
                Name = "Name",
                Description = new string('A', 226),
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task CreateGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateGameCommand
            {
                Name = "Test",
                Description = "Desc"
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateGameCommand_AdminNoImage_CreateRecord()
        {
            // Arrange
            SetupAdminUser();

            var command = new CreateGameCommand
            {
                Name = "Test Game",
                Description = "This is a test game"
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var game = await DbContext.Games.FindAsync(result.Id);
            game.Should().NotBeNull();
            game.PictureId.Should().BeNull();
            game.Name.Should().Be(command.Name);
            game.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task CreateGameCommand_AdminWithImage_CreateRecord()
        {
            // Arrange
            SetupAdminUser();

            var command = new CreateGameCommand
            {
                Name = "Test Game with Image",
                Description = "This is a test game with image",
                Image = CreateTestImage(),
                AltText = "Test Image Alt Text"
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var game = await DbContext.Games.FindAsync(result.Id);
            game.Should().NotBeNull();
            game.PictureId.Should().NotBeNull();
            game.Name.Should().Be(command.Name);
            game.Description.Should().Be(command.Description);

            var picture = await DbContext.Pictures.FindAsync(game.PictureId);
            picture.Should().NotBeNull();
            picture.AltText.Should().Be(command.AltText);
            picture.UploadedBy.Should().Be(1);

        }

        /// <summary>
        /// ///////////////////////////////////////////////////// Update /////////////////////////////////
        /// </summary>
        [Fact]
        public async Task UpdateGame_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            var id = await AddRecord();

            var command = new UpdateGameCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
                Id = id
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task UpdateGame_NameEmpty_ThrowError()
        {
            // Arrange
            var id = await AddRecord();

            var command = new UpdateGameCommand
            {
                Name = "",
                Description = "Desc",
                Id = id
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task UpdateGame_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            var id = await AddRecord();

            var command = new UpdateGameCommand
            {
                Name = "Name",
                Description = new string('A', 226),
                Id = id
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task UpdateGame_GameIdEmpty_ThrowError()
        {
            // Arrange
            var command = new UpdateGameCommand
            {
                Name = "Name",
                Description = "Des",
                Id = 0
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }


        [Fact]
        public async Task UpdateGame_InvalidId_ThrowError()
        {
            // Arrange
            SetupAdminUser();
            var command = new UpdateGameCommand
            {
                Name = "Test Save",
                Description = "Desc",
                Id = 9999999
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid Game");
        }

        [Fact]
        public async Task UpdateGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateGameCommand
            {
                Name = "Test",
                Description = "Desc"
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateGameCommand_Admin_CreateRecord()
        {
            // Arrange
            SetupAdminUser();

            var gameId = await AddRecord();
            var command = new UpdateGameCommand
            {
                Id = gameId,
                Name = "Update Game",
                Description = "This is a test game"
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            var game = await DbContext.Games.FindAsync(gameId);
            game.Should().NotBeNull();
            game.PictureId.Should().BeNull();
            game.Name.Should().Be(command.Name);
            game.Description.Should().Be(command.Description);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////// Delete /////////////////////////////////
        /// </summary>
        [Fact]
        public async Task DeleteGame_NotFoundId_ThrowError()
        {
            // Arrange
            SetupAdminUser();
            var command = new DeleteGameCommand(Id: IncorrectValue);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid Game");
        }

        [Fact]
        public async Task DeleteGame_IdEmpty_ThrowError()
        {
            // Arrange
            var command = new DeleteGameCommand(Id: 0);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteGameCommand(Id: 1);

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteGameCommand_AdminNoImage_DeleteRecord()
        {
            // Arrange
            SetupAdminUser();

            var gameId = await AddRecord();

            var command = new DeleteGameCommand(Id: gameId);

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            var game = await DbContext.Games.FindAsync(gameId);
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
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

    }
}
