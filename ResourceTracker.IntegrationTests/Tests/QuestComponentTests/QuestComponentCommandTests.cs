using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponents;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.QuestComponentTests
{
    public class QuestComponentCommandTests : IntegrationTestBase
    {
        public QuestComponentCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        private int _gameId;
        private int _gameSaveId;
        private int _questId;
        private int _componentId;

        protected override async Task ClassSetup()
        {
            _gameId = await AddGameRecord();
            _gameSaveId = await AddGameSaveRecord(_gameId);
            _questId = await AddQuestRecord(_gameSaveId);
            _componentId = await AddComponentRecord(_gameId);
        }

        /// Validators - Create single
        [Fact]
        public async Task CreateQuestComponent_AmountAquiredInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentCommand
            {
                AmountAquired = 0,
                ComponentId = _componentId,
                QuestId = _questId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Amount aquired cannot be less than 1");
        }

        [Fact]
        public async Task CreateQuestComponent_ComponentIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentCommand
            {
                AmountAquired = 1,
                ComponentId = 0,
                QuestId = _questId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Component is required");
        }

        [Fact]
        public async Task CreateQuestComponent_QuestIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentCommand
            {
                AmountAquired = 1,
                ComponentId = _componentId,
                QuestId = 0
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Quest is required");
        }

        [Fact]
        public async Task CreateQuestComponent_InvalidUser_ThrowError()
        {
            // Arrange
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            var command = new CreateQuestComponentCommand
            {
                AmountAquired = 1,
                ComponentId = _componentId,
                QuestId = _questId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task CreateQuestComponent_ValidRequest_CreateRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentCommand
            {
                AmountAquired = 5,
                ComponentId = _componentId,
                QuestId = _questId
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var entity = await DbContext.QuestComponents.FindAsync(result.Id);
            entity.Should().NotBeNull();
            entity!.AmountAquired.Should().Be(command.AmountAquired);
            entity.ComponentId.Should().Be(command.ComponentId);
            entity.QuestId.Should().Be(command.QuestId);
        }

        /// Create multiple
        [Fact]
        public async Task CreateQuestComponents_QuestIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentsCommand
            {
                QuestId = 0,
                Commands = new List<CreateQuestComponentClass>()
                {
                    new CreateQuestComponentClass { AmountAquired = 1, ComponentId = _componentId }
                }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Quest is required");
        }

        [Fact]
        public async Task CreateQuestComponents_CommandItemInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentsCommand
            {
                QuestId = _questId,
                Commands = new List<CreateQuestComponentClass>()
                {
                    new CreateQuestComponentClass { AmountAquired = -1, ComponentId = _componentId }
                }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Amount aquired cannot be less than 1");
        }

        [Fact]
        public async Task CreateQuestComponents_ValidRequest_CreateRecords()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateQuestComponentsCommand
            {
                QuestId = _questId,
                Commands = new List<CreateQuestComponentClass>()
                {
                    new CreateQuestComponentClass { AmountAquired = 2, ComponentId = _componentId },
                    new CreateQuestComponentClass { AmountAquired = 3, ComponentId = _componentId }
                }
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        /// Update single
        [Fact]
        public async Task UpdateQuestComponent_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateQuestComponentCommand
            {
                Id = 0,
                AmountAquired = 1
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task UpdateQuestComponent_AmountInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateQuestComponentCommand
            {
                Id = 1,
                AmountAquired = -1
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Amount aquired cannot be less than 1");
        }

        [Fact]
        public async Task UpdateQuestComponent_InvalidUser_ThrowError()
        {
            // Arrange
            var existingId = await AddQuestComponentRecord(_questId, _componentId);

            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            var command = new UpdateQuestComponentCommand
            {
                Id = existingId,
                AmountAquired = 10
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task UpdateQuestComponent_InvalidId_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateQuestComponentCommand
            {
                Id = IncorrectValue,
                AmountAquired = 10
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (QuestComponents) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task UpdateQuestComponent_ValidRequest_UpdateRecord()
        {
            // Arrange
            var id = await AddQuestComponentRecord(_questId, _componentId);
            SetupNonAdminUser();

            var command = new UpdateQuestComponentCommand
            {
                Id = id,
                AmountAquired = 7
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.QuestComponents.FindAsync(id);
            entity.Should().NotBeNull();
            entity!.AmountAquired.Should().Be(command.AmountAquired);
        }

        /// Update multiple
        [Fact]
        public async Task UpdateQuestComponents_EmptyCommands_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateQuestComponentsCommand
            {
                Commands = new List<UpdateQuestComponentCommand>()
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Atleast 1 command is required");
        }

        [Fact]
        public async Task UpdateQuestComponents_CommandItemNotFound_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateQuestComponentsCommand
            {
                Commands = new List<UpdateQuestComponentCommand>()
                {
                    new UpdateQuestComponentCommand { Id = IncorrectValue, AmountAquired = 5 }
                }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (QuestComponents) with Key ({command.Commands.First().Id}) was not found.");
        }

        [Fact]
        public async Task UpdateQuestComponents_ValidRequest_BulkUpdate()
        {
            // Arrange
            var id1 = await AddQuestComponentRecord(_questId, _componentId);
            var id2 = await AddQuestComponentRecord(_questId, _componentId);

            SetupNonAdminUser();

            var command = new UpdateQuestComponentsCommand
            {
                Commands = new List<UpdateQuestComponentCommand>()
                {
                    new UpdateQuestComponentCommand { Id = id1, AmountAquired = 11 },
                    new UpdateQuestComponentCommand { Id = id2, AmountAquired = 0 }   
                }
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var e1 = await DbContext.QuestComponents.FindAsync(id1);
            var e2 = await DbContext.QuestComponents.FindAsync(id2);
            e1.Should().NotBeNull();
            e2.Should().NotBeNull();
            e1!.AmountAquired.Should().Be(11);
            e2!.AmountAquired.Should().Be(0);
        }

        /// Delete
        [Fact]
        public async Task DeleteQuestComponent_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteQuestComponentCommand(Id: 0);

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteQuestComponent_NotFound_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteQuestComponentCommand(Id: IncorrectValue);

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (QuestComponents) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task DeleteQuestComponent_ValidData_DeleteRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var id = await AddQuestComponentRecord(_questId, _componentId);

            var command = new DeleteQuestComponentCommand(Id: id);

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.QuestComponents.FindAsync(id);
            entity.Should().BeNull();
        }

        #region Helpers
        private async Task<int> AddGameRecord()
        {
            var game = new Game
            {
                Name = "Existing Game",
                Description = "Existing Description"
            };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

        private async Task<int> AddGameSaveRecord(int _gameId)
        {
            var gs = new GameSave
            {
                Name = "Existing Game Save",
                Description = "Existing Description",
                GameId = _gameId,
                Created = DateTime.UtcNow,
                UserId = NonAdminUserId
            };
            DbContext.GameSaves.Add(gs);
            await DbContext.SaveChangesAsync();
            return gs.Id;
        }

        private async Task<int> AddQuestRecord(int _gameSaveId)
        {
            var quest = new Quest
            {
                Name = "Existing Quest",
                Description = "Existing Description",
                Location = "Existing Location",
                GameSaveId = _gameSaveId
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            return quest.Id;
        }

        private async Task<int> AddComponentRecord(int _gameId)
        {
            var component = new Component
            {
                Name = "Existing Component",
                Description = "Existing Description",
                GameId = _gameId,
                Type = 1
            };
            DbContext.Components.Add(component);
            await DbContext.SaveChangesAsync();
            return component.Id;
        }

        private async Task<int> AddQuestComponentRecord(int _questId, int _componentId, int amount = 1)
        {
            var qc = new QuestComponents
            {
                AmountAquired = amount,
                QuestId = _questId,
                ComponentId = _componentId
            };
            DbContext.QuestComponents.Add(qc);
            await DbContext.SaveChangesAsync();
            return qc.Id;
        }
        #endregion
    }
}