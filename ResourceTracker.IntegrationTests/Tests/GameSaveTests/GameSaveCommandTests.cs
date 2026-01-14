

using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.CreateGameSave;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.DeleteGameSave;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.UpdateGameSave;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.GameSaveTests
{
    public class GameSaveCommandTests : IntegrationTestBase
    {
        private int gameId;
        private int userId = 2;

        public GameSaveCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            gameId = await AddGameRecord();
        }

        [Fact]
        public async Task CreateGameSave_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var command = new CreateGameSaveCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
                GameId = gameId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task CreateGameSave_NameEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var command = new CreateGameSaveCommand
            {
                Name = "",
                Description = "Desc",
                GameId = gameId
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateGameSave_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var command = new CreateGameSaveCommand
            {
                Name = "Name",
                Description = new string('A', 226),
                GameId = gameId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task CreateGameSave_GameIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var command = new CreateGameSaveCommand
            {
                Name = "Name",
                Description = "Des",
                GameId = 0
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid game selected");
        }

        [Fact]
        public async Task CreateGameSave_InvalidUser_ThrowError()
        {
            // Arrange
            UserInfoMock
            .Setup(x => x.GetUserId())
            .Returns(0);

            var command = new CreateGameSaveCommand
            {
                Name = "Test Save",
                Description = "Desc",
                GameId = gameId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");

        }


        [Fact]
        public async Task CreateGameSave_ValidRequest_CreateRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateGameSaveCommand
            {
                Name = "Test Save",
                Description = "Desc",
                GameId = gameId
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var entity = await DbContext.GameSaves.FindAsync(result.Id);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
            entity.GameId.Should().Be(command.GameId);
            entity.UserId.Should().Be(userId);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////// Update /////////////////////////////////
        /// </summary>
        [Fact]
        public async Task UpdateGameSave_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var id = await AddRecord();

            var command = new UpdateGameSaveCommand
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
        public async Task UpdateGameSave_NameEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var id = await AddRecord();

            var command = new UpdateGameSaveCommand
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
        public async Task UpdateGameSave_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var id = await AddRecord();

            var command = new UpdateGameSaveCommand
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
        public async Task UpdateGameSave_GameSaveIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateGameSaveCommand
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
        public async Task UpdateGameSave_InvalidUser_ThrowError()
        {
            // Arrange
            var id = await AddRecord();
            UserInfoMock
            .Setup(x => x.GetUserId())
            .Returns(0);

            var command = new UpdateGameSaveCommand
            {
                Name = "Test Save",
                Description = "Desc",
                Id = id
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("You do not have access to this content");
        }

        [Fact]
        public async Task UpdateGameSave_InvalidId_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateGameSaveCommand
            {
                Name = "Test Save",
                Description = "Desc",
                Id = 9999999
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid GameSave");
        }


        [Fact]
        public async Task UpdateGameSave_ValidRequest_UpdateRecord()
        {
            // Arrange
            var id = await AddRecord();
            SetupNonAdminUser();

            var command = new UpdateGameSaveCommand
            {
                Name = "Test Save",
                Description = "Desc",
                Id = id
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert

            var entity = await DbContext.GameSaves.FindAsync(command.Id);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
            entity.UserId.Should().Be(userId);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////// Delete /////////////////////////////////
        /// </summary>
        [Fact]
        public async Task DeleteGameSave_NotFoundId_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteGameSaveCommand(Id: IncorrectValue);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid GameSave");
        }

        [Fact]
        public async Task DeleteGameSave_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteGameSaveCommand(Id: 0);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteGameSaveCommand_ValidData_DeleteRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var gameId = await AddRecord();

            var command = new DeleteGameSaveCommand(Id : gameId);

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.GameSaves.FindAsync(gameId);
            entity.Should().BeNull();
        }


        private async Task<int> AddRecord()
        {
            var game = new GameSave
            {
                Name = "Existing Game Save",
                Description = "Existing Description",
                GameId = gameId,
                Created = DateTime.UtcNow,
                UserId = userId
                
            };
            DbContext.GameSaves.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }
        private async Task<int> AddGameRecord()
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
