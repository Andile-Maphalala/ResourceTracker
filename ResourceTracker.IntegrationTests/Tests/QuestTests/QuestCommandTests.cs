using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.QuestTests
{
    public class QuestCommandTests : IntegrationTestBase
    {
        private int gameId;
        private int gameSaveId;
        public QuestCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            gameId = await AddGameRecord();
            gameSaveId = await AddGameSaveRecord(gameId);
        }

        [Fact]
        public async Task CreateQuest_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            CreateQuestCommand command = new CreateQuestCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task CreateQuest_NameEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            CreateQuestCommand command = new CreateQuestCommand
            {
                Name = "",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateQuest_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            CreateQuestCommand command = new CreateQuestCommand
            {
                Name = "Name",
                Description = new string('A', 226),
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task CreateQuest_GameSaveIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            CreateQuestCommand command = new CreateQuestCommand
            {
                Name = "Name",
                Description = "Des",
                Location = "Loc",
                GameSaveId = 0
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Game Save is required");
        }

        [Fact]
        public async Task CreateQuest_InvalidUser_ThrowError()
        {
            // Arrange
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            CreateQuestCommand command = new CreateQuestCommand
            {
                Name = "Test Quest",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task CreateQuest_ValidRequest_CreateRecord()
        {
            // Arrange
            SetupNonAdminUser();
            CreateQuestCommand command = new CreateQuestCommand
            {
                Name = "Test Quest",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            CreateQuestResponse result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            Quest entity = await DbContext.Quests.FindAsync(result.Id);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
            entity.Location.Should().Be(command.Location);
            entity.GameSaveId.Should().Be(command.GameSaveId);
        }

        /// <summary>
        /// Update tests
        /// </summary>
        [Fact]
        public async Task UpdateQuest_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddRecord();

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = id,
                Name = new string('A', 101),
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task UpdateQuest_NameEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddRecord();

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = id,
                Name = "",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task UpdateQuest_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddRecord();

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = id,
                Name = "Name",
                Description = new string('A', 226),
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task UpdateQuest_QuestIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = 0,
                Name = "Name",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task UpdateQuest_InvalidUser_ThrowError()
        {
            // Arrange
            int id = await AddRecord();
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = id,
                Name = "Test Quest",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task UpdateQuest_InvalidId_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = 9999999,
                Name = "Test Quest",
                Description = "Desc",
                Location = "Loc",
                GameSaveId = gameSaveId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (Quest) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task UpdateQuest_ValidRequest_UpdateRecord()
        {
            // Arrange
            int id = await AddRecord();
            SetupNonAdminUser();

            UpdateQuestCommand command = new UpdateQuestCommand
            {
                Id = id,
                Name = "Updated Quest",
                Description = "Updated Desc",
                Location = "Updated Loc",
                GameSaveId = gameSaveId
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            Quest entity = await DbContext.Quests.FindAsync(command.Id);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
            entity.Location.Should().Be(command.Location);
        }

        /// <summary>
        /// Delete tests
        /// </summary>
        [Fact]
        public async Task DeleteQuest_NotFoundId_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            DeleteQuestCommand command = new DeleteQuestCommand(Id: IncorrectValue);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (Quest) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task DeleteQuest_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            DeleteQuestCommand command = new DeleteQuestCommand(Id: 0);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteQuestCommand_ValidData_DeleteRecord()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddRecord();

            DeleteQuestCommand command = new DeleteQuestCommand(Id: id);

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            Quest entity = await DbContext.Quests.FindAsync(id);
            entity.Should().BeNull();
        }

        private async Task<int> AddRecord()
        {
            Quest quest = new Quest
            {
                Name = "Existing Quest",
                Description = "Existing Description",
                Location = "Existing Location",
                GameSaveId = gameSaveId
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            return quest.Id;
        }

        private async Task<int> AddGameSaveRecord(int gameId)
        {
            GameSave gs = new GameSave
            {
                Name = "Existing Game Save",
                Description = "Existing Description",
                GameId = gameId,
                Created = DateTime.UtcNow,
                UserId = NonAdminUserId
            };
            DbContext.GameSaves.Add(gs);
            await DbContext.SaveChangesAsync();
            return gs.Id;
        }

        private async Task<int> AddGameRecord()
        {
            Game game = new Game
            {
                Name = "Existing Game",
                Description = "Existing Description"
            };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }
    }
}