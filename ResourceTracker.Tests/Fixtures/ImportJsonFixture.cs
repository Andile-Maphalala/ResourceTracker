

using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;
using System.Text.Json;

namespace ResourceTracker.Tests.Fixtures
{
    public class ImportJsonFixture<T>
    {
        public T Data { get; }

        protected virtual string FileName => throw new NotImplementedException();

        protected virtual string GameName => throw new NotImplementedException();

        public ImportJsonFixture()
        {
            var path = Path.Combine("TestData", "ImportSamples", GameName, FileName);
            var json = File.ReadAllText(path);
            Data = JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException("Failed to deserialize test JSON.");
        }

        
    }
}
