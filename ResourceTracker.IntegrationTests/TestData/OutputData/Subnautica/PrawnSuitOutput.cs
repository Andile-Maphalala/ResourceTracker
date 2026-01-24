
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.IntegrationTests.TestData.OutputData.Subnautica
{
    public static class PrawnSuitOutput
    {
        public static List<BuildPlanComponentRequirementDto> ExpectedResponseDataNoFacilityNoInvestory ()
        {
            return new List<BuildPlanComponentRequirementDto>
            {
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Quartz",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 1,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Titanium",
                    RequiredAmount = 20,
                    MissingAmount = 20,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
            };
        }

        public static List<BuildPlanComponentRequirementDto> ExpectedResponseDataNoFacilityWithInvestoryOnlyResources()
        {
            return new List<BuildPlanComponentRequirementDto>
            {
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 1,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Quartz",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 1,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Titanium",
                    RequiredAmount = 20,
                    MissingAmount = 7,
                    AvailableAmount = 13,
                    Type = (int)ComponentTypeEnum.Resource
                }
            };
        }

        public static List<BuildPlanComponentRequirementDto> ExpectedResponseDataNoFacilityWithInvestoryCompositeAndResources()
        {
            return new List<BuildPlanComponentRequirementDto>
            {
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 1,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Quartz",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 0,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 1,
                    ComponentName = "Titanium",
                    RequiredAmount = 20,
                    MissingAmount = 7,
                    AvailableAmount = 13,
                    Type = (int)ComponentTypeEnum.Resource
                }
            };
        }


    }
}
