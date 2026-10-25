using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Commands.GameCommands.PurgeGame;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using ResourceTracker.IntegrationTests.Setup;

namespace ResourceTracker.IntegrationTests.Tests.GameTests
{
    public class PurgeGameTests : IntegrationTestBase, IDisposable
    {
        private int _gameId;
        private int _otherGameId;
        private int _gameSaveId;
        private int _questId;
        private int _componentId;
        private int _facilityId;
        private int _buildPlanId;

        private readonly List<string> _imagePaths = new();

        public PurgeGameTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override async Task ClassSetup()
        {
            SetupAdminUser();

            // Pictures first, so the game, a component and a quest can each point at one.
            var gamePicture = await AddPicture("game.png");
            var componentPicture = await AddPicture("component.png");
            var questPicture = await AddPicture("quest.png");

            _gameId = await AddGame("Purge Me", gamePicture);
            _otherGameId = await AddGame("Survivor", null);

            _gameSaveId = await AddGameSave(_gameId);
            _questId = await AddQuest(_gameSaveId, questPicture);

            _componentId = await AddComponent(_gameId, "Iron", ComponentTypeEnum.Resource, componentPicture);
            _facilityId = await AddComponent(_gameId, "Forge", ComponentTypeEnum.Facility, null);

            await AddRecipe(_facilityId, _componentId);
            await AddQuestComponent(_componentId, _questId);
            await AddPlayerFacility(_facilityId, _questId);

            _buildPlanId = await AddBuildPlan(_gameSaveId);
            await AddBuildPlanComponent(_componentId, _buildPlanId);
            await AddBuildPlanQuest(_buildPlanId, _questId);

            // A second game with its own save, quest and component, to prove it survives.
            var otherSaveId = await AddGameSave(_otherGameId);
            await AddQuest(otherSaveId, null);
            await AddComponent(_otherGameId, "Keep Me", ComponentTypeEnum.Resource, null);
        }

        [Fact]
        public async Task Purge_DeletesTheGameAndEverythingItOwns()
        {
            var response = await Sender.Send(new PurgeGameCommand(_gameId, "Purge Me"), CancellationToken.None);

            response.GameId.Should().Be(_gameId);
            response.ComponentsDeleted.Should().Be(2);
            response.RecipesDeleted.Should().Be(1);
            response.RecipeComponentsDeleted.Should().Be(1);
            response.InventoryRowsDeleted.Should().Be(1);
            response.PlayerFacilitiesDeleted.Should().Be(1);
            response.QuestsDeleted.Should().Be(1);
            response.GameSavesDeleted.Should().Be(1);
            response.BuildPlansDeleted.Should().Be(1);
            response.BuildPlanComponentsDeleted.Should().Be(1);
            response.BuildPlanQuestsDeleted.Should().Be(1);
            response.PicturesDeleted.Should().Be(3);
            response.ImageFilesDeleted.Should().Be(3);

            (await DbContext.Games.CountAsync(g => g.Id == _gameId)).Should().Be(0);
            (await DbContext.Components.CountAsync(c => c.GameId == _gameId)).Should().Be(0);
            (await DbContext.GameSaves.CountAsync(gs => gs.GameId == _gameId)).Should().Be(0);
            (await DbContext.Quests.CountAsync(q => q.GameSaveId == _gameSaveId)).Should().Be(0);
            (await DbContext.Recipes.CountAsync(r => r.Component.GameId == _gameId)).Should().Be(0);
            (await DbContext.QuestComponents.CountAsync(qc => qc.QuestId == _questId)).Should().Be(0);
            (await DbContext.BuildPlans.CountAsync(bp => bp.GameSaveId == _gameSaveId)).Should().Be(0);
            (await DbContext.Pictures.CountAsync()).Should().Be(0);

            _imagePaths.Should().OnlyContain(path => !File.Exists(path));
        }

        [Fact]
        public async Task Purge_LeavesOtherGamesUntouched()
        {
            await Sender.Send(new PurgeGameCommand(_gameId, "Purge Me"), CancellationToken.None);

            (await DbContext.Games.CountAsync(g => g.Id == _otherGameId)).Should().Be(1);
            (await DbContext.Components.CountAsync(c => c.GameId == _otherGameId)).Should().Be(1);
            (await DbContext.GameSaves.CountAsync(gs => gs.GameId == _otherGameId)).Should().Be(1);
            (await DbContext.Quests.CountAsync(q => q.GameSave.GameId == _otherGameId)).Should().Be(1);
        }

        [Fact]
        public async Task Purge_WrongConfirmName_ThrowsAndChangesNothing()
        {
            var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(new PurgeGameCommand(_gameId, "Purge Mee"), CancellationToken.None));

            ex.Message.Should().Contain("ConfirmName");

            (await DbContext.Games.CountAsync(g => g.Id == _gameId)).Should().Be(1);
            (await DbContext.Components.CountAsync(c => c.GameId == _gameId)).Should().Be(2);
            (await DbContext.Pictures.CountAsync()).Should().Be(3);
            _imagePaths.Should().OnlyContain(path => File.Exists(path));
        }

        [Fact]
        public async Task Purge_NonAdmin_ThrowsAndChangesNothing()
        {
            SetupNonAdminUser();

            await Assert.ThrowsAsync<BadRequestException>(() =>
                Sender.Send(new PurgeGameCommand(_gameId, "Purge Me"), CancellationToken.None));

            (await DbContext.Games.CountAsync(g => g.Id == _gameId)).Should().Be(1);
            (await DbContext.Components.CountAsync(c => c.GameId == _gameId)).Should().Be(2);
        }

        [Fact]
        public async Task Purge_UnknownGame_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                Sender.Send(new PurgeGameCommand(IncorrectValue, "whatever"), CancellationToken.None));
        }

        public void Dispose()
        {
            foreach (var path in _imagePaths.Where(File.Exists))
            {
                try { File.Delete(path); } catch { /* best effort */ }
            }
        }

        #region helpers

        private async Task<int> AddPicture(string fileName)
        {
            var path = Path.Combine(Path.GetTempPath(), $"purge-{Guid.NewGuid()}-{fileName}");
            await File.WriteAllBytesAsync(path, new byte[] { 1, 2, 3 });
            _imagePaths.Add(path);

            var picture = new Picture
            {
                Path = path,
                Name = fileName,
                ContentType = "image/png",
                Size = 3,
                AltText = fileName,
                CreatedDate = DateTime.UtcNow
            };

            DbContext.Pictures.Add(picture);
            await DbContext.SaveChangesAsync();
            return picture.Id;
        }

        private async Task<int> AddGame(string name, int? pictureId)
        {
            var game = new Game { Name = name, Description = "Purge tests", PictureId = pictureId };
            DbContext.Games.Add(game);
            await DbContext.SaveChangesAsync();
            return game.Id;
        }

        private async Task<int> AddGameSave(int gameId)
        {
            var gameSave = new GameSave
            {
                Name = "Purge Test Save",
                Description = "Purge tests",
                Created = DateTime.UtcNow,
                UserId = NonAdminUserId,
                GameId = gameId
            };
            DbContext.GameSaves.Add(gameSave);
            await DbContext.SaveChangesAsync();
            return gameSave.Id;
        }

        private async Task<int> AddQuest(int gameSaveId, int? pictureId)
        {
            var quest = new Quest
            {
                Name = "Purge Test Quest",
                Description = "Purge tests",
                Location = "Purge Test Location",
                GameSaveId = gameSaveId,
                PictureId = pictureId
            };
            DbContext.Quests.Add(quest);
            await DbContext.SaveChangesAsync();
            return quest.Id;
        }

        private async Task<int> AddComponent(int gameId, string name, ComponentTypeEnum type, int? pictureId)
        {
            var component = new Component
            {
                Name = name,
                Description = "Purge tests",
                GameId = gameId,
                Type = (int)type,
                PictureId = pictureId
            };
            DbContext.Components.Add(component);
            await DbContext.SaveChangesAsync();
            return component.Id;
        }

        private async Task AddRecipe(int outputComponentId, int ingredientComponentId)
        {
            var recipe = new Recipe
            {
                ComponentId = outputComponentId,
                AmountMade = 1,
                RecipeComponents = new List<RecipeComponent>
                {
                    new RecipeComponent { ComponentId = ingredientComponentId, AmountRequired = 1 }
                }
            };
            DbContext.Recipes.Add(recipe);
            await DbContext.SaveChangesAsync();
        }

        private async Task AddQuestComponent(int componentId, int questId)
        {
            DbContext.QuestComponents.Add(new QuestComponents
            {
                AmountAquired = 5,
                ComponentId = componentId,
                QuestId = questId
            });
            await DbContext.SaveChangesAsync();
        }

        private async Task AddPlayerFacility(int componentId, int questId)
        {
            DbContext.Set<PlayerFacility>().Add(new PlayerFacility
            {
                ComponentId = componentId,
                QuestId = questId,
                GameSaveId = _gameSaveId,
                Level = 1
            });
            await DbContext.SaveChangesAsync();
        }

        private async Task<int> AddBuildPlan(int gameSaveId)
        {
            var buildPlan = new BuildPlan
            {
                Name = "Purge Test Plan",
                Description = "Purge tests",
                GameSaveId = gameSaveId
            };
            DbContext.BuildPlans.Add(buildPlan);
            await DbContext.SaveChangesAsync();
            return buildPlan.Id;
        }

        private async Task AddBuildPlanComponent(int componentId, int buildPlanId)
        {
            DbContext.BuildPlanComponents.Add(new BuildPlanComponent
            {
                Order = 1,
                QuantityNeeded = 1,
                ComponentId = componentId,
                BuildPlanId = buildPlanId
            });
            await DbContext.SaveChangesAsync();
        }

        private async Task AddBuildPlanQuest(int buildPlanId, int questId)
        {
            DbContext.BuildPlanQuests.Add(new BuildPlanQuest
            {
                BuildPlanId = buildPlanId,
                QuestId = questId
            });
            await DbContext.SaveChangesAsync();
        }

        #endregion
    }
}
