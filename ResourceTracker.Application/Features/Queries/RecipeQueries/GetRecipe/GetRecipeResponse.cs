namespace ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipe
{
    public class GetRecipeResponse
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string? ComponentImageUrl { get; set; }
        public int AmountMade { get; set; }

        // The station this is crafted at. Both null and level 1 when the recipe has no station.
        public int? RequiredFacilityId { get; set; }
        public string? RequiredFacilityName { get; set; }
        public string? RequiredFacilityImageUrl { get; set; }
        public int RequiredFacilityLevel { get; set; }

        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientResponse
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int ComponentType { get; set; }
        public string ComponentTypeName { get; set; }
        public int AmountRequired { get; set; }
        public string? ComponentImageUrl { get; set; }
    }
}
