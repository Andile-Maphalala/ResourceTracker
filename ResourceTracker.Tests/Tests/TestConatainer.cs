

using ResourceTracker.Tests.Fixtures;

namespace ResourceTracker.Tests.Tests
{
    public class TestConatainer : IClassFixture<IntegrationTestFixture>
    {

        private readonly IntegrationTestFixture _fixture;
        public TestConatainer(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void SampleTest()
        {
            Assert.True(true);
        }
    }
}
