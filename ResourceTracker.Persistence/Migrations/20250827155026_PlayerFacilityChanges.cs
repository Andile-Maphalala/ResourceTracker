using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PlayerFacilityChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlan_GameSave_GameSaveId",
                table: "BuildPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlanComponent_Component_ComponentId",
                table: "BuildPlanComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlanQuest_BuildPlan_BuildPlanId",
                table: "BuildPlanQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlanQuest_Quest_QuestId",
                table: "BuildPlanQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSave_User_UserId",
                table: "GameSave");

            migrationBuilder.DropForeignKey(
                name: "FK_Quest_GameSave_GameSaveId",
                table: "Quest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestComponents_Component_ComponentId",
                table: "QuestComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Component_ComponentId",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeComponent_Component_ComponentId",
                table: "RecipeComponent");

            migrationBuilder.AddColumn<int>(
                name: "RequiredFacilityId",
                table: "Recipe",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlayerFacility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameSaveId = table.Column<int>(type: "integer", nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFacility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerFacility_Component_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerFacility_GameSave_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerFacility_Quest_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_RequiredFacilityId",
                table: "Recipe",
                column: "RequiredFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFacility_ComponentId",
                table: "PlayerFacility",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFacility_GameSaveId",
                table: "PlayerFacility",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFacility_QuestId",
                table: "PlayerFacility",
                column: "QuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlan_GameSave_GameSaveId",
                table: "BuildPlan",
                column: "GameSaveId",
                principalTable: "GameSave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlanComponent_Component_ComponentId",
                table: "BuildPlanComponent",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlanQuest_BuildPlan_BuildPlanId",
                table: "BuildPlanQuest",
                column: "BuildPlanId",
                principalTable: "BuildPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlanQuest_Quest_QuestId",
                table: "BuildPlanQuest",
                column: "QuestId",
                principalTable: "Quest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSave_User_UserId",
                table: "GameSave",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_GameSave_GameSaveId",
                table: "Quest",
                column: "GameSaveId",
                principalTable: "GameSave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestComponents_Component_ComponentId",
                table: "QuestComponents",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Component_ComponentId",
                table: "Recipe",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Component_RequiredFacilityId",
                table: "Recipe",
                column: "RequiredFacilityId",
                principalTable: "Component",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeComponent_Component_ComponentId",
                table: "RecipeComponent",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlan_GameSave_GameSaveId",
                table: "BuildPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlanComponent_Component_ComponentId",
                table: "BuildPlanComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlanQuest_BuildPlan_BuildPlanId",
                table: "BuildPlanQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlanQuest_Quest_QuestId",
                table: "BuildPlanQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSave_User_UserId",
                table: "GameSave");

            migrationBuilder.DropForeignKey(
                name: "FK_Quest_GameSave_GameSaveId",
                table: "Quest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestComponents_Component_ComponentId",
                table: "QuestComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Component_ComponentId",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Component_RequiredFacilityId",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeComponent_Component_ComponentId",
                table: "RecipeComponent");

            migrationBuilder.DropTable(
                name: "PlayerFacility");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_RequiredFacilityId",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "RequiredFacilityId",
                table: "Recipe");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlan_GameSave_GameSaveId",
                table: "BuildPlan",
                column: "GameSaveId",
                principalTable: "GameSave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlanComponent_Component_ComponentId",
                table: "BuildPlanComponent",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlanQuest_BuildPlan_BuildPlanId",
                table: "BuildPlanQuest",
                column: "BuildPlanId",
                principalTable: "BuildPlan",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlanQuest_Quest_QuestId",
                table: "BuildPlanQuest",
                column: "QuestId",
                principalTable: "Quest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSave_User_UserId",
                table: "GameSave",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_GameSave_GameSaveId",
                table: "Quest",
                column: "GameSaveId",
                principalTable: "GameSave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestComponents_Component_ComponentId",
                table: "QuestComponents",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Component_ComponentId",
                table: "Recipe",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeComponent_Component_ComponentId",
                table: "RecipeComponent",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
