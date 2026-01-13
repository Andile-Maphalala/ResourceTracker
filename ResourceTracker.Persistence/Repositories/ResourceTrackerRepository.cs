using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Data;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;

namespace ResourceTracker.Persistence.Repositories
{
    public class ResourceTrackerRepository : GenericRepository, IResourceTrackerRepository
    {
        private readonly IGameQueryBuilder _gameQueryBuilder;
        private readonly IComponetQueryBuilder _componentQueryBuilder;
        private readonly IQuestQueryBuilder _questQueryBuilder;
        private readonly IQuestComponetsQueryBuilder _questComponetsQueryBuilder;

        public ResourceTrackerRepository(ResourceTrackerDbContext context, IGameQueryBuilder gameQueryBuilder, IComponetQueryBuilder componentQueryBuilder, IQuestQueryBuilder questQueryBuilder, IQuestComponetsQueryBuilder questComponetsQueryBuilder) : base(context)
        {
            _gameQueryBuilder = gameQueryBuilder;
            _componentQueryBuilder = componentQueryBuilder;
            _questQueryBuilder = questQueryBuilder;
            _questComponetsQueryBuilder = questComponetsQueryBuilder;
        }

        public IQueryable<User> Users  => Set<User>();
        public IQueryable<Quest> Quests  => Set<Quest>();
        public IQueryable<Component> Components  => Set<Component>();
        public IQueryable<Recipe> Recipes  => Set<Recipe>();
        public IQueryable<QuestComponents> QuestComponents  => Set<QuestComponents>();
        public IQueryable<RecipeComponent> RecipeComponents => Set<RecipeComponent>();
        public IQueryable<BuildPlan> BuildPlans => Set<BuildPlan>();
        public IQueryable<BuildPlanComponent> BuildPlanComponents => Set<BuildPlanComponent>();
        public IQueryable<Game> Games => Set<Game>();
        public IQueryable<Picture> Pictures => Set<Picture>();
        public IQueryable<GameSave> GameSaves => Set<GameSave>();
        public IQueryable<BuildPlanQuest> BuildPlanQuests => Set<BuildPlanQuest>();

        public IQueryable<Game> Search(SearchGamesQuery request)
        {
            return _gameQueryBuilder.ApplyFilters(Games, request);
        }

        public IQueryable<Component> Search(SearchComponentsQuery request)
        {
            return _componentQueryBuilder.ApplyFilters(Components, request);
        }

        public IQueryable<QuestComponents> Search(SearchQuestComponentsQuery request)
        {
            return _questComponetsQueryBuilder.ApplyFilters(QuestComponents, request);
        }

        public IQueryable<Quest> Search(SearchQuestsQuery request)
        {
            return _questQueryBuilder.ApplyFilters(Quests, request);
        }
    }
}
