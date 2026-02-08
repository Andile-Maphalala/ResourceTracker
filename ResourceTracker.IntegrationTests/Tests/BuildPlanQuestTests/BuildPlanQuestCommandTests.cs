using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponents;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponents;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponents;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanBuildPlanTests
{
    public class BuildPlanBuildPlanCommandTests : IntegrationTestBase
    {
        public BuildPlanBuildPlanCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {

        }

        private int _gameId;
        private int _gameSaveId;
        private int _buildPlanId;
        private int _componentId;

        protected override async Task ClassSetup()
        {
            _gameId = await AddGameRecord();
            _gameSaveId = await AddGameSaveRecord(_gameId);
            _buildPlanId = await AddBuildPlanRecord(_gameSaveId);
            _componentId = await AddComponentRecord(_gameId);
        }
        /// Validators - Create single
        [Fact]
        public async Task CreateBuildPlanComponent_QuantityNeededInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentCommand
            {
                QuantityNeeded = -1,
                ComponentId = _componentId,
                BuildPlanId = _buildPlanId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Quantity must be greater than 0");
        }

        [Fact]
        public async Task CreateBuildPlanComponent_ComponentIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentCommand
            {
                QuantityNeeded = 1,
                ComponentId = 0,
                BuildPlanId = _buildPlanId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Component is required");
        }

        [Fact]
        public async Task CreateBuildPlanComponent_BuildPlanIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentCommand
            {
                QuantityNeeded = 1,
                ComponentId = _componentId,
                BuildPlanId = 0
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("BuildPlan is required");
        }

        [Fact]
        public async Task CreateBuildPlanComponent_InvalidUser_ThrowError()
        {
            // Arrange
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            var command = new CreateBuildPlanComponentCommand
            {
                QuantityNeeded = 1,
                ComponentId = _componentId,
                BuildPlanId = _buildPlanId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task CreateBuildPlanComponent_ValidRequest_CreateRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentCommand
            {
                QuantityNeeded = 5,
                ComponentId = _componentId,
                BuildPlanId = _buildPlanId
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var entity = await DbContext.BuildPlanComponents.FindAsync(result.Id);
            entity.Should().NotBeNull();
            entity!.QuantityNeeded.Should().Be(command.QuantityNeeded);
            entity.ComponentId.Should().Be(command.ComponentId);
            entity.BuildPlanId.Should().Be(command.BuildPlanId);
        }

        /// Create multiple
        [Fact]
        public async Task CreateBuildPlanComponents_BuildPlanIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentsCommand
            {
                BuildPlanId = 0,
                Commands = new List<CreateBuildPlanComponentDto>()
                {
                    new CreateBuildPlanComponentDto { QuantityNeeded = 1, ComponentId = _componentId }
                }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("BuildPlan is required");
        }

        [Fact]
        public async Task CreateBuildPlanComponents_CommandItemInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentsCommand
            {
                BuildPlanId = _buildPlanId,
                Commands = new List<CreateBuildPlanComponentDto>()
                {
                    new CreateBuildPlanComponentDto { QuantityNeeded = -1, ComponentId = _componentId }
                }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Quantity must be greater than 0");
        }

        [Fact]
        public async Task CreateBuildPlanComponents_ValidRequest_CreateRecords()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanComponentsCommand
            {
                BuildPlanId = _buildPlanId,
                Commands = new List<CreateBuildPlanComponentDto>()
                {
                    new CreateBuildPlanComponentDto { QuantityNeeded = 2, ComponentId = _componentId },
                    new CreateBuildPlanComponentDto { QuantityNeeded = 3, ComponentId = _componentId }
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
        public async Task UpdateBuildPlanComponent_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentCommand
            {
                Id = 0,
                QuantityNeeded = 1
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task UpdateBuildPlanComponent_AmountInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentCommand
            {
                Id = 1,
                QuantityNeeded = -1
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Quantity must be greater than 0");
        }

        [Fact]
        public async Task UpdateBuildPlanComponent_InvalidUser_ThrowError()
        {
            // Arrange
            var existingId = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);

            SetUser(0);

            var command = new UpdateBuildPlanComponentCommand
            {
                Id = existingId,
                QuantityNeeded = 10
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task UpdateBuildPlanComponent_InvalidId_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentCommand
            {
                Id = IncorrectValue,
                QuantityNeeded = 10
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlanComponent) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task UpdateBuildPlanComponent_ValidRequest_UpdateRecord()
        {
            // Arrange
            var id = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);
            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentCommand
            {
                Id = id,
                QuantityNeeded = 7
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.BuildPlanComponents.FindAsync(id);
            entity.Should().NotBeNull();
            entity!.QuantityNeeded.Should().Be(command.QuantityNeeded);
        }

        /// Update multiple
        [Fact]
        public async Task UpdateBuildPlanComponents_EmptyCommands_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentsCommand
            {
                Commands = new List<UpdateBuildPlanComponentCommand>()
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Atleast 1 command is required");
        }

        [Fact]
        public async Task UpdateBuildPlanComponents_CommandItemNotFound_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentsCommand
            {
                Commands = new List<UpdateBuildPlanComponentCommand>()
                {
                    new UpdateBuildPlanComponentCommand { Id = IncorrectValue, QuantityNeeded = 5 }
                }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlanComponent) with Key ({command.Commands.First().Id}) was not found.");
        }

        [Fact]
        public async Task UpdateBuildPlanComponents_ValidRequest_BulkUpdate()
        {
            // Arrange
            var id1 = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);
            var id2 = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);

            SetupNonAdminUser();

            var command = new UpdateBuildPlanComponentsCommand
            {
                Commands = new List<UpdateBuildPlanComponentCommand>()
                {
                    new UpdateBuildPlanComponentCommand { Id = id1, QuantityNeeded = 11 },
                    new UpdateBuildPlanComponentCommand { Id = id2, QuantityNeeded = 1 }
                }
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var e1 = await DbContext.BuildPlanComponents.FindAsync(id1);
            var e2 = await DbContext.BuildPlanComponents.FindAsync(id2);
            e1.Should().NotBeNull();
            e2.Should().NotBeNull();
            e1!.QuantityNeeded.Should().Be(11);
            e2!.QuantityNeeded.Should().Be(1);
        }

        /// Delete
        [Fact]
        public async Task DeleteBuildPlanComponent_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanComponentCommand(Id: 0);

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteBuildPlanComponent_NotFound_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanComponentCommand(Id: IncorrectValue);

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlanComponent) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task DeleteBuildPlanComponent_ValidData_DeleteRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var id = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);

            var command = new DeleteBuildPlanComponentCommand(Id: id);

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.BuildPlanComponents.FindAsync(id);
            entity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBuildPlanComponents_EmptyIds_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanComponentsCommand
            {
                Ids = new List<int>()
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Atleast 1 id is required");
        }

        [Fact]
        public async Task DeleteBuildPlanComponents_IdItemInvalid_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanComponentsCommand
            {
                Ids = new List<int> { 0 }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteBuildPlanComponents_CommandItemNotFound_ThrowNotFound()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanComponentsCommand
            {
                Ids = new List<int> { IncorrectValue }
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlanComponent) with Key ({command.Ids.First()}) was not found.");
        }

        [Fact]
        public async Task DeleteBuildPlanComponents_ValidRequest_BulkDelete()
        {
            // Arrange
            var id1 = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);
            var id2 = await AddBuildPlanComponentRecord(_buildPlanId, _componentId);

            SetupNonAdminUser();

            var command = new DeleteBuildPlanComponentsCommand
            {
                Ids = new List<int> { id1, id2 }
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var e1 = await DbContext.BuildPlanComponents.FindAsync(id1);
            var e2 = await DbContext.BuildPlanComponents.FindAsync(id2);
            e1.Should().BeNull();
            e2.Should().BeNull();
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

        private async Task<int> AddBuildPlanRecord(int _gameSaveId)
        {
            var quest = new BuildPlan
            {
                Name = "Existing BuildPlan",
                Description = "Existing Description",
                GameSaveId = _gameSaveId
            };
            DbContext.BuildPlans.Add(quest);
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

        private async Task<int> AddBuildPlanComponentRecord(int _buildPlanId, int _componentId, int amount = 1)
        {
            var qc = new BuildPlanComponent
            {
                QuantityNeeded = amount,
                BuildPlanId = _buildPlanId,
                ComponentId = _componentId
            };
            DbContext.BuildPlanComponents.Add(qc);
            await DbContext.SaveChangesAsync();
            return qc.Id;
        }
        #endregion
    }
}