namespace ResourceTracker.Application.Features.Commands.GameCommands.PurgeGame
{
    public class PurgeGameResponse
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public int ComponentsDeleted { get; set; }
        public int RecipesDeleted { get; set; }
        public int RecipeComponentsDeleted { get; set; }
        public int InventoryRowsDeleted { get; set; }
        public int PlayerFacilitiesDeleted { get; set; }
        public int QuestsDeleted { get; set; }
        public int GameSavesDeleted { get; set; }
        public int BuildPlansDeleted { get; set; }
        public int BuildPlanComponentsDeleted { get; set; }
        public int BuildPlanQuestsDeleted { get; set; }
        public int PicturesDeleted { get; set; }
        public int ImageFilesDeleted { get; set; }
    }
}
