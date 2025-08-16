using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Data;

namespace ResourceTracker.Persistence.Repositories
{
    public class ResourceTrackerRepository : GenericRepository, IResourceTrackerRepository
    {

        public ResourceTrackerRepository(ResourceTrackerDbContext context) : base(context)
        {

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
    }
}
