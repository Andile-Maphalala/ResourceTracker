using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.IntegrationTests
{
    public class UnitTest1 : IntegrationTestBase
    {
        [Fact]
        public async Task Get_CreateHandler_ReturnsSuccess()
        {
            // Arrange
            // Seed test data if needed
            var testEntity = new Game { Name = "Test", Description = "testing" };
            var handler = _serviceProvider.GetRequiredService<IRequestHandler<GetGameQuery, GetGameResponse>>();
            await _dbContext.Games.AddAsync(testEntity);
            await _dbContext.SaveChangesAsync();

            // Act
            var response = await handler.Handle(new GetGameQuery { Id = 1 }, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
        }
    }
}
