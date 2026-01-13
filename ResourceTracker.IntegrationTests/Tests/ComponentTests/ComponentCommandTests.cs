using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.ComponentTests
{
    public class ComponentCommandTests : IntegrationTestBase
    {
        private int _gameId;
        public ComponentCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {

        }

        protected override async Task ClassSetup()
        {
            var game = new Game
            {
                Name = "Existing Game",
                Description = "Existing Description",
            };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            _gameId = game.Id;
        }

        [Fact]
        public async Task CreateomponentCommand_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            var command = new CreateComponentCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
                Type = (int)ComponentTypeEnum.Resource,
                GameId = _gameId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task CreateomponentCommand_NameEmpty_ThrowError()
        {
            // Arrange
            var command = new CreateComponentCommand
            {
                Name = "",
                Description = "Desc",
                Type = (int)ComponentTypeEnum.Resource,
                GameId = _gameId
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateomponentCommand_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            var command = new CreateComponentCommand
            {
                Name = "Name",
                Description = new string('A', 226),
                Type = (int)ComponentTypeEnum.Resource,
                GameId = _gameId
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task CreateomponentCommand_TypeEmpty_ThrowError()
        {
            // Arrange
            var command = new CreateComponentCommand
            {
                Name = "Name",
                Description = "Desc",
                GameId = _gameId
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Type is required");
        }

        [Fact]
        public async Task CreateomponentCommand_TypeInvalidEnum_ThrowError()
        {
            // Arrange
            var command = new CreateComponentCommand
            {
                Name = "Name",
                Description = "Desc",
                Type = IncorrectValue,
                GameId = _gameId
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Type is not a valid component type");
        }

        [Fact]
        public async Task CreateComponentCommand_NonAdmin_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateComponentCommand
            {
                Name = "Test",
                Description = "Desc",
                Type = (int)ComponentTypeEnum.Resource,
                GameId = _gameId
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateComponentCommand_Admin_CreateRecord()
        {
            // Arrange
            SetupAdminUser();

            var command = new CreateComponentCommand
            {
                Name = "Test Component",
                Description = "This is a test",
                Type = (int)ComponentTypeEnum.Resource,
                GameId = _gameId
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var entity = await DbContext.Components.FindAsync(result.Id);
            entity.Should().NotBeNull();
            entity.PictureId.Should().BeNull();
            entity.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
            entity.Type.Should().Be(command.Type);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////// Update /////////////////////////////////
        /// </summary>
        [Fact]
        public async Task UpdateomponentCommand_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            var id = await AddRecord();

            var command = new UpdateComponentCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
                Id = id,
                Type = (int)ComponentTypeEnum.Resource
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task UpdateomponentCommand_NameEmpty_ThrowError()
        {
            // Arrange
            var id = await AddRecord();

            var command = new UpdateComponentCommand
            {
                Name = "",
                Description = "Desc",
                Id = id,
                Type = (int)ComponentTypeEnum.Resource
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task UpdateomponentCommand_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            var id = await AddRecord();

            var command = new UpdateComponentCommand
            {
                Name = "Name",
                Description = new string('A', 226),
                Id = id,
                Type = (int)ComponentTypeEnum.Composite
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task UpdateomponentCommand_ComponentIdEmpty_ThrowError()
        {
            // Arrange
            var command = new UpdateComponentCommand
            {
                Name = "Name",
                Description = "Des",
                Type = (int)ComponentTypeEnum.Composite
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }


        [Fact]
        public async Task UpdateComponentCommand_InvalidId_ThrowError()
        {
            // Arrange
            SetupAdminUser();
            var command = new UpdateComponentCommand
            {
                Name = "Test Save",
                Description = "Desc",
                Id = IncorrectValue,
                Type = (int)ComponentTypeEnum.Composite
            };

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (Component) with Key ({command.Id}) was not found.");

        }

        [Fact]
        public async Task UpdateComponentCommand_TypeEmpty_ThrowError()
        {
            // Arrange
            var id = await AddRecord();
            var command = new UpdateComponentCommand
            {
                Id = id,
                Name = "Name",
                Description = "Desc",
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Type is required");
        }

        [Fact]
        public async Task UpdateComponentCommand_TypeInvalidEnum_ThrowError()
        {
            // Arrange
            var id = await AddRecord();
            var command = new UpdateComponentCommand
            {
                Id = id,
                Name = "Name",
                Description = "Desc",
                Type = IncorrectValue
            };
            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Type is not a valid component type");
        }

        [Fact]
        public async Task UpdateComponentCommand_NonAdmin_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            var id = await AddRecord();
            var command = new UpdateComponentCommand
            {
                Id = id,
                Name = "Test",
                Description = "Desc",
                Type = (int)ComponentTypeEnum.Composite
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateComponentCommand_Admin_UpdateRecord()
        {
            // Arrange
            SetupAdminUser();

            var entityId = await AddRecord();
            var command = new UpdateComponentCommand
            {
                Id = entityId,
                Name = "Update Component",
                Description = "This is a test entity",
                Type = (int)ComponentTypeEnum.Composite
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.Components.FindAsync(entityId);
            entity.Should().NotBeNull();
            entity.PictureId.Should().BeNull();
            entity.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////// Delete /////////////////////////////////
        /// </summary>
        [Fact]
        public async Task DeleteomponentCommand_NotFoundId_ThrowError()
        {
            // Arrange
            SetupAdminUser();
            var command = new DeleteComponentCommand(Id: IncorrectValue);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (Component) with Key ({command.Id}) was not found.");

        }

        [Fact]
        public async Task DeleteomponentCommand_IdEmpty_ThrowError()
        {
            // Arrange
            var command = new DeleteComponentCommand(Id: 0);

            // Act
            // Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteComponentCommand_NonAdmin_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteComponentCommand(Id: 1);

            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteComponentCommand_AdminNoImage_DeleteRecord()
        {
            // Arrange
            SetupAdminUser();

            var entityId = await AddRecord();

            var command = new DeleteComponentCommand(Id: entityId);

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.Components.FindAsync(entityId);
            entity.Should().BeNull();
        }

        private async Task<int> AddRecord()
        {
            var entity = new Component
            {
                Name = "Existing Component",
                Description = "Existing Description",
                Type = (int)ComponentTypeEnum.Resource,
                GameId = _gameId
            };
            DbContext.Components.Add(entity);
            await DbContext.SaveChangesAsync();
            return entity.Id;
        }
    }
}