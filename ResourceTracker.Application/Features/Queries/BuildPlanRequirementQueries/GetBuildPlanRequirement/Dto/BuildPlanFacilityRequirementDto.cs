
namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto
{
    public class BuildPlanFacilityRequirementDto
    {
        public int FacilityId { get; set; }
        public string Name { get; set; }

        // The highest level any recipe in this plan demands of this facility.
        public int RequiredLevel { get; set; }

        // The best level the player owns among the plan's locations; 0 means not owned there.
        // With RequiredLevel this gives three states, which IsAvailable alone cannot express:
        //   OwnedLevel == 0                    -> build one
        //   0 < OwnedLevel < RequiredLevel     -> built, but needs upgrading
        //   OwnedLevel >= RequiredLevel        -> satisfied
        public int OwnedLevel { get; set; }

        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public List<int> RequiredFor { get; set; }
    }
}
