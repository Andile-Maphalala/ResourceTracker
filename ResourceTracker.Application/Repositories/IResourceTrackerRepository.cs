using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Repositories
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
        IQueryable<BuildPlanResource> BuildPlanResources { get; }
        IQueryable<Game> Games { get; }


    }
}
