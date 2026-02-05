
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
                    ComponentId = 30,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 24,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 31,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 22,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 29,
                    ComponentName = "Quartz",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 25,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 27,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 1,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 14,
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
                    ComponentId = 30,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 1,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 24,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 31,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 22,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 29,
                    ComponentName = "Quartz",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 25,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 27,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 1,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 14,
                    ComponentName = "Titanium",
                    RequiredAmount = 20,
                    MissingAmount = 7,
                    AvailableAmount = 13,
                    Type = (int)ComponentTypeEnum.Resource
                }
            };
        }

        public static List<BuildPlanComponentRequirementDto> ExpectedResponseDataNoFacilityWithInvestoryCompositeAndResources_EnameldGlass()
        {
            return new List<BuildPlanComponentRequirementDto>
            {
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 30,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 1,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 24,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 31,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 22,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 25,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 14,
                    ComponentName = "Titanium",
                    RequiredAmount = 20,
                    MissingAmount = 7,
                    AvailableAmount = 13,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 26,
                    ComponentName = "Enameled Glass",
                    RequiredAmount = 1,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Composite
                },
            };
        }

        public static List<BuildPlanComponentRequirementDto> ExpectedResponseDataNoFacilityWithInvestoryCompositeAndResources_Glass()
        {
            return new List<BuildPlanComponentRequirementDto>
            {
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 30,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 1,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 24,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 31,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 22,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 25,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 27,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 1,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 14,
                    ComponentName = "Titanium",
                    RequiredAmount = 20,
                    MissingAmount = 7,
                    AvailableAmount = 13,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 28,
                    ComponentName = "Glass",
                    RequiredAmount = 1,
                    MissingAmount = 0,
                    AvailableAmount = 3,
                    Type = (int)ComponentTypeEnum.Composite
                },
            };
        }

        public static List<BuildPlanComponentRequirementDto> ExpectedResponseDataWithFacilityyWithInvestoryCompositeAndResources_Glass()
        {
            return new List<BuildPlanComponentRequirementDto>
            {
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 30,
                    ComponentName = "Diamond",
                    RequiredAmount = 2,
                    MissingAmount = 1,
                    AvailableAmount = 1,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 24,
                    ComponentName = "Gel Sack",
                    RequiredAmount = 2,
                    MissingAmount = 0,
                    AvailableAmount = 2,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 31,
                    ComponentName = "Lead",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 22,
                    ComponentName = "Lithium",
                    RequiredAmount = 4,
                    MissingAmount = 4,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                },
                new BuildPlanComponentRequirementDto
                {
                    ComponentId = 25,
                    ComponentName = "Ruby",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 27,
                    ComponentName = "Stalker Tooth",
                    RequiredAmount = 1,
                    MissingAmount = 1,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 14,
                    ComponentName = "Titanium",
                    RequiredAmount = 20 + 11,//for the facility requirements
                    MissingAmount = 7 + 11 ,//for the facility requirements
                    AvailableAmount = 13,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 28,
                    ComponentName = "Glass",
                    RequiredAmount = 1,
                    MissingAmount = 0,
                    AvailableAmount = 3,
                    Type = (int)ComponentTypeEnum.Composite
                }
                //for the facility requirements
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 11,
                    ComponentName = "Copper Ore",
                    RequiredAmount = 5,
                    MissingAmount = 5,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 9,
                    ComponentName = "Gold",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 8,
                    ComponentName = "Table Coral Sample",
                    RequiredAmount = 3,
                    MissingAmount = 3,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 6,
                    ComponentName = "Silver Ore",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 13,
                    ComponentName = "Acid Mushroom",
                    RequiredAmount = 6,
                    MissingAmount = 6,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
                ,new BuildPlanComponentRequirementDto
                {
                    ComponentId = 18,
                    ComponentName = "Creepvine Seed Cluster",
                    RequiredAmount = 2,
                    MissingAmount = 2,
                    AvailableAmount = 0,
                    Type = (int)ComponentTypeEnum.Resource
                }
            };
        }

        public static List<BuildPlanFacilityRequirementDto> ExpectedFacilityWithNoneInvestory()
        {
            return new List<BuildPlanFacilityRequirementDto>
            {
                new BuildPlanFacilityRequirementDto
                {
                    FacilityId = 3,
                    Name = "Fabricator",
                    IsAvailable = false,
                }
                ,new BuildPlanFacilityRequirementDto
                {
                    FacilityId = 2,
                    Name = "Mobile Vehicle Bay",
                    IsAvailable = false,
                }
                 ,new BuildPlanFacilityRequirementDto
                {
                    FacilityId = 4,
                    Name = "Habitat Builder",
                    IsAvailable = false,
                }
            };

        }
    }
}
