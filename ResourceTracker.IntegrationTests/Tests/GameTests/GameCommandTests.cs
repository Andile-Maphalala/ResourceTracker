

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;

namespace ResourceTracker.IntegrationTests.Tests.GameTests
{

    public class GameCommandTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateGameCommand_NonAdmin_ThrowError()
        {
            // Arrange
            var handler = _serviceProvider.GetRequiredService<IRequestHandler<CreateGameCommand, CreateGameResponse>>();
            var command = new CreateGameCommand
            {
                Name = "Test Game",
                Description = "This is a test game"
            };
            // Act
            // Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>handler.Handle(command, CancellationToken.None));
        }
    }
}
