
namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto
{
    public class BuildPlanFacilityRequirementDto
    {
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public List<int> RequiredFor { get; set; }
    }
}
