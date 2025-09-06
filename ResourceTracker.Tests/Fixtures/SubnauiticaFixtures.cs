using ResourceTracker.Application.Features.Commands.ImportCommands.Dtos;


namespace ResourceTracker.Tests.Fixtures
{

    public class SubnauticaFixture<T> : ImportJsonFixture<T>
    {
        protected override string GameName => "Subnautica";
    }
    public class PrawnSuitFixture<T> : SubnauticaFixture<T>
    {
        protected override string FileName => "PrawnSuit.json";
    }

    public class AllComponentsFixture<T> : SubnauticaFixture<T>
    {
        protected override string FileName => "AllComponents.json";
    }
}
