

namespace ResourceTracker.Application.Features.Queries.BuildPlanQuestQueries.GetBuildPlanQuestList
{
    public class GetBuildPlanQuestListResponse
    {
        public int BuildPlanId { get; set; }
        public string BuildPlanName { get; set; }
        public string BuildPlanDescription { get; set; }
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public string QuestDescription { get; set; }
    }
}
