using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Interfaces
{
    public interface IResourceTrackerRepository : IGenericRepository
    {
        IQueryable<User> Users { get; }
        IQueryable<Quest> Quests { get; }
        IQueryable<Component> Components { get; }
        IQueryable<Recipe> Recipes { get; }
        IQueryable<QuestComponents> QuestComponents { get; }
        IQueryable<RecipeComponent> RecipeComponents { get; }
        IQueryable<BuildPlan> BuildPlans { get; }
        IQueryable<BuildPlanComponent> BuildPlanComponents { get; }
        IQueryable<Game> Games { get; }
        IQueryable<Picture> Pictures { get; }
        IQueryable<GameSave> GameSaves { get; }
        IQueryable<BuildPlanQuest> BuildPlanQuests { get; }

        IQueryable<Game> Search(SearchGamesQuery request);
        IQueryable<Component> Search(SearchComponentsQuery request);
        IQueryable<QuestComponents> Search(SearchQuestComponentsQuery request);
        IQueryable<Quest> Search(SearchQuestsQuery request);
    }
}
