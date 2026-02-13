using FluentAssertions;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.CreateBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.DeleteBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.UpdateBuildPlan;
using ResourceTracker.Domain.Entities;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.BuildPlanTests
{
    public class BuildPlanCommandTests : IntegrationTestBase
    {
        private int gameId;
        private int gameSaveId;
        public BuildPlanCommandTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            gameId = await AddGameRecord();
            gameSaveId = await AddGameSaveRecord(gameId);
        }

        [Fact]
        public async Task CreateBuildPlan_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanCommand
            {
                Name = new string('A', 101),
                Description = "Desc",
                GameSaveId = gameSaveId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task CreateBuildPlan_NameEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanCommand
            {
                Name = "",
                Description = "Desc",
                GameSaveId = gameSaveId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateBuildPlan_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanCommand
            {
                Name = "Name",
                Description = new string('A', 226),
                GameSaveId = gameSaveId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task CreateBuildPlan_GameSaveIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanCommand
            {
                Name = "Name",
                Description = "Desc",
                GameSaveId = 0
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Game Save is required");
        }

        [Fact]
        public async Task CreateBuildPlan_InvalidUser_ThrowError()
        {
            // Arrange
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            var command = new CreateBuildPlanCommand
            {
                Name = "Test BuildPlan",
                Description = "Desc",
                GameSaveId = gameSaveId
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task CreateBuildPlan_ValidRequest_CreateRecord()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new CreateBuildPlanCommand
            {
                Name = "Test BuildPlan",
                Description = "Desc",
                GameSaveId = gameSaveId
            };

            // Act
            var result = await Sender.Send(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var entity = await DbContext.BuildPlans.FindAsync(result.Id);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
            entity.GameSaveId.Should().Be(command.GameSaveId);
        }

        /// Update tests
        [Fact]
        public async Task UpdateBuildPlan_NameExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddBuildPlanRecord(gameSaveId);

            var command = new UpdateBuildPlanCommand
            {
                Id = id,
                Name = new string('A', 101),
                Description = "Desc"
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name cannot exceed 100 characters");
        }

        [Fact]
        public async Task UpdateBuildPlan_NameEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddBuildPlanRecord(gameSaveId);

            var command = new UpdateBuildPlanCommand
            {
                Id = id,
                Name = "",
                Description = "Desc"
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task UpdateBuildPlan_DescriptionExceedMaximumLength_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddBuildPlanRecord(gameSaveId);

            var command = new UpdateBuildPlanCommand
            {
                Id = id,
                Name = "Name",
                Description = new string('A', 226)
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Description cannot exceed 225 characters");
        }

        [Fact]
        public async Task UpdateBuildPlan_BuildPlanIdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanCommand
            {
                Id = 0,
                Name = "Name",
                Description = "Desc"
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task UpdateBuildPlan_InvalidUser_ThrowError()
        {
            // Arrange
            int id = await AddBuildPlanRecord(gameSaveId);
            UserInfoMock
                .Setup(x => x.GetUserId())
                .Returns(0);

            var command = new UpdateBuildPlanCommand
            {
                Id = id,
                Name = "Test BuildPlan",
                Description = "Desc"
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Invalid User");
        }

        [Fact]
        public async Task UpdateBuildPlan_InvalidId_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new UpdateBuildPlanCommand
            {
                Id = IncorrectValue,
                Name = "Test BuildPlan",
                Description = "Desc"
            };

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlan) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task UpdateBuildPlan_ValidRequest_UpdateRecord()
        {
            // Arrange
            int id = await AddBuildPlanRecord(gameSaveId);
            SetupNonAdminUser();

            var command = new UpdateBuildPlanCommand
            {
                Id = id,
                Name = "Updated BuildPlan",
                Description = "Updated Desc"
            };

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.BuildPlans.FindAsync(command.Id);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(command.Name);
            entity.Description.Should().Be(command.Description);
        }

        /// Delete tests
        [Fact]
        public async Task DeleteBuildPlan_NotFoundId_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanCommand(Id: IncorrectValue);

            // Act / Assert
            var result = await Assert.ThrowsAsync<NotFoundException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be($"Entity (BuildPlan) with Key ({command.Id}) was not found.");
        }

        [Fact]
        public async Task DeleteBuildPlan_IdEmpty_ThrowError()
        {
            // Arrange
            SetupNonAdminUser();

            var command = new DeleteBuildPlanCommand(Id: 0);

            // Act / Assert
            var result = await Assert.ThrowsAsync<BadRequestException>(() => Sender.Send(command, CancellationToken.None));
            result.Message.Should().Be("Id is required");
        }

        [Fact]
        public async Task DeleteBuildPlan_ValidData_DeleteRecord()
        {
            // Arrange
            SetupNonAdminUser();
            int id = await AddBuildPlanRecord(gameSaveId);

            var command = new DeleteBuildPlanCommand(Id: id);

            // Act
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var entity = await DbContext.BuildPlans.FindAsync(id);
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

        private async Task<int> AddGameSaveRecord(int gameId)
        {
            var gs = new GameSave
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

        private async Task<int> AddBuildPlanRecord(int gameSaveId)
        {
            var bp = new BuildPlan
            {
                Name = "Existing BuildPlan",
                Description = "Existing Description",
                GameSaveId = gameSaveId
            };
            DbContext.BuildPlans.Add(bp);
            await DbContext.SaveChangesAsync();
            return bp.Id;
        }
        #endregion
    }
}
