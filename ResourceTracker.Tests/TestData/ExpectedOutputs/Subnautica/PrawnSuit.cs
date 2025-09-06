using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using System.Collections.ObjectModel;


namespace ResourceTracker.Tests.TestData.ExpectedOutputs.Subnautica
{
    public static class PrawnSuit
    {
        public static List<Component> ExpectedComponentData()
        {
            return new List<Component>
            {
                new Component
                {
                   Id = 1,
                   Name = "Prawn Suit",
                   Description = "Epic mech suit designed for navigating challenging environments on foot.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 2,
                   Name = "Mobile Vehicle Bay",
                   Description = "Fabricates vehicles from raw materials.",
                   Type = (int)ComponentTypeEnum.Facility,
                },
                new Component
                {
                   Id = 3,
                   Name = "Fabricator",
                   Description = "Basic survival fabricator. Atomically rearranges raw resources into useful objects.",
                   Type = (int)ComponentTypeEnum.Facility,
                },
                new Component
                {
                   Id = 4,
                   Name = "Habitat Builder",
                   Description = "Fabricates habitat compartments and appliances from raw materials.",
                   Type = (int)ComponentTypeEnum.Facility,
                },
                new Component
                {
                   Id = 5,
                   Name = "Wiring Kit",
                   Description = "Insulated silver wire. Essential electronic component.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 6,
                   Name = "Silver Ore",
                   Description = "Ag. Conductive element and microbicide.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 7,
                   Name = "Computer Chip",
                   Description = "Multi-purpose CPU.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 8,
                   Name = "Table Coral Sample",
                   Description = "Contains trace precious metals used in computer fabrication.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 9,
                   Name = "Gold",
                   Description = "Au. Valuable conductive properties. Highly valuable social property.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 10,
                   Name = "Copper Wire",
                   Description = "Copper atoms fabricated into basic electrical wiring.",
                   Type = (int)ComponentTypeEnum.Composite,
                },

                new Component
                {
                   Id = 11,
                   Name = "Copper Ore",
                   Description = "Cu. Essential wiring component.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 12,
                   Name = "Battery",
                   Description = "Mobile power source.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 13,
                   Name = "Acid Mushroom",
                   Description = "Purple fungus. Acidic flesh.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 14,
                   Name = "Titanium",
                   Description = "Ti. Basic building material.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 15,
                   Name = "Metal Salvage",
                   Description = "Composed primarily of titanium.",
                   Type = (int)ComponentTypeEnum.Resource,

                },
                new Component
                {
                   Id = 16,
                   Name = "Titanium Ingot",
                   Description = "Ti. Condensed titanium bar.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 17,
                   Name = "Lubricant",
                   Description = "Naturally-derived, oil-based lubricant. Industrial applications.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 18,
                   Name = "Creepvine Seed Cluster",
                   Description = "Indigenous seeds with high silicone and oil content.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 19,
                   Name = "Power Cell",
                   Description = "High-capacity mobile power source.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 20,
                   Name = "Silicone Rubber",
                   Description = "Synthetic, silicone-based rubber.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 21,
                   Name = "Plasteel Ingot",
                   Description = "Ultra-strong synthetic construction material.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 22,
                   Name = "Lithium",
                   Description = "Li. Applications in high-strength alloys.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 23,
                   Name = "Aerogel",
                   Description = "Light, porous, dried gel. High heat insulation.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 24,
                   Name = "Gel Sack",
                   Description = "Gel sack.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 25,
                   Name = "Ruby",
                   Type = (int)ComponentTypeEnum.Resource,

                },
                new Component
                {
                   Id = 26,
                   Name = "Enameled Glass",
                   Description = "Glass, hardened using a natural substrate.",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 27,
                   Name = "Stalker Tooth",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 28,
                   Name = "Glass",
                   Type = (int)ComponentTypeEnum.Composite,
                },
                new Component
                {
                   Id = 29,
                   Name = "Quartz",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 30,
                   Name = "Diamond",
                   Description = "C. Carbon allotrope with superlative physical properties.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
                new Component
                {
                   Id = 31,
                   Name = "Lead",
                   Description = "Pb. Insulates against radiation.",
                   Type = (int)ComponentTypeEnum.Resource,
                },
            };
        }

        public static List<Recipe> ExpectedRecipeData()
        {
            return new List<Recipe>()
            {
                new Recipe
                {
                    Id = 1,
                    AmountMade = 1,
                    ComponentId = 1, // Prawn Suit
                    RequiredFacilityId = 2, // Mobile Vehicle Bay
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 21, AmountRequired = 2 }, // Plasteel Ingot
                        new RecipeComponent { ComponentId = 23, AmountRequired = 2 }, // Aerogel
                        new RecipeComponent { ComponentId = 26, AmountRequired = 1 }, // Enameled Glass
                        new RecipeComponent { ComponentId = 29, AmountRequired = 2 },// Diamond
                        new RecipeComponent { ComponentId = 30, AmountRequired = 2 }, // Lead
                    }
                },
                new Recipe
                {
                    Id = 2,
                    AmountMade = 1,
                    ComponentId = 2, //Mobile Vehicle Bay
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 16, AmountRequired = 1 }, // Titanium Ingot
                        new RecipeComponent { ComponentId = 17, AmountRequired = 1 }, // Lubricant
                        new RecipeComponent { ComponentId = 19, AmountRequired = 1 }, // Power Cell
                    }
                },
                new Recipe
                {
                    Id = 3,
                    AmountMade = 1,
                    ComponentId = 3, // Fabricator
                    RequiredFacilityId = 4, // Habitat Builder
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 5, AmountRequired = 1 }, // Wiring Kit
                        new RecipeComponent { ComponentId = 7, AmountRequired = 1 }, // Computer Chip
                        new RecipeComponent { ComponentId = 12, AmountRequired = 1 }, // Battery
                    }
                },
                new Recipe
                {
                    Id = 4,
                    AmountMade = 1,
                    ComponentId = 4, // Habitat Builder
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 5, AmountRequired = 1 }, // Wiring Kit
                        new RecipeComponent { ComponentId = 7, AmountRequired = 1 }, // Computer Chip
                        new RecipeComponent { ComponentId = 12, AmountRequired = 1 }, // Battery

                    }
                },
                new Recipe
                {
                    Id = 5,
                    AmountMade = 1,
                    ComponentId = 5, // Wiring Kit
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 6, AmountRequired = 2 }, // Silver Ore
                    }
                },
                new Recipe
                {
                    Id = 6,
                    AmountMade = 1,
                    ComponentId = 7, // Computer Chip
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 8, AmountRequired = 2 }, // Table Coral Sample
                        new RecipeComponent { ComponentId = 9, AmountRequired = 1 }, // Gold
                        new RecipeComponent { ComponentId = 10, AmountRequired = 1 }, // Copper Wire
                    }
                },
                new Recipe
                {
                    Id = 7,
                    AmountMade = 1,
                    ComponentId = 10, // Copper Wire
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 11, AmountRequired = 2 }, // Copper Ore
                    }
                },
                new Recipe
                {
                    Id = 8,
                    AmountMade = 1,
                    ComponentId = 12, // Battery
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 13, AmountRequired = 2 }, // Acid Mushroom
                        new RecipeComponent { ComponentId = 14, AmountRequired = 1 }, // Titanium
                    }
                },
                new Recipe {
                    Id = 9,
                    AmountMade = 4,
                    ComponentId = 14, // Titanium
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 15, AmountRequired = 1 }, // Metal Salvage
                    }
                },
                new Recipe
                {
                    Id = 10,
                    AmountMade = 1,
                    ComponentId = 16, // Titanium Ingot
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 14, AmountRequired = 10 }, // Titanium
                    }
                },
                new Recipe
                {
                    Id = 11,
                    AmountMade = 1,
                    ComponentId = 17, // Lubricant
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 18, AmountRequired = 1 }, // Creepvine Seed Cluster
                    }
                },
                new Recipe
                {
                    Id = 12,
                    AmountMade = 1,
                    ComponentId = 19, // Power Cell
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 12, AmountRequired = 2 }, // Battery
                        new RecipeComponent { ComponentId = 20, AmountRequired = 1 }, // Silicone Rubber
                    }
                },
                new Recipe
                {
                    Id = 13,
                    AmountMade = 2,
                    ComponentId = 20, // Silicone Rubber
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 18, AmountRequired = 1 }, // Creepvine Seed Cluster
                    }
                },
                new Recipe
                {
                    Id = 14,
                    AmountMade = 1,
                    ComponentId = 21, // Plasteel Ingot
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 16, AmountRequired = 1 }, // Titanium Ingot
                        new RecipeComponent { ComponentId = 22, AmountRequired = 2 }, // Lithium
                    }
                },
                new Recipe
                {
                    Id = 15,
                    AmountMade = 1,
                    ComponentId = 23, // Aerogel
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 24, AmountRequired = 1 }, // Gel Sack
                        new RecipeComponent { ComponentId = 25, AmountRequired = 1 }, // Ruby
                    }
                },
                new Recipe
                {
                    Id = 16,
                    AmountMade = 1,
                    ComponentId = 26, // Enameled Glass
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 27, AmountRequired = 1 }, // Stalker Tooth
                        new RecipeComponent { ComponentId = 28, AmountRequired = 1 }, // Glass
                    }
                },
                new Recipe
                {
                    Id = 17,
                    AmountMade = 1,
                    ComponentId = 28, // Glass
                    RequiredFacilityId = 3, // Fabricator
                    RecipeComponents = new Collection<RecipeComponent>()
                    {
                        new RecipeComponent { ComponentId = 29, AmountRequired = 1 }, // Quartz
                    }
                },
            };
        }
    }
}
