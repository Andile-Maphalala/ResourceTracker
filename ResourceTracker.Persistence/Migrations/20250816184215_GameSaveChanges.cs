using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GameSaveChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlan_Game_GameId",
                table: "BuildPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildPlan_User_UserId",
                table: "BuildPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Quest_Game_GameId",
                table: "Quest");

            migrationBuilder.DropForeignKey(
                name: "FK_Quest_User_UserId",
                table: "Quest");

            migrationBuilder.DropTable(
                name: "BuildPlanResource");

            migrationBuilder.DropIndex(
                name: "IX_Quest_GameId",
                table: "Quest");

            migrationBuilder.DropIndex(
                name: "IX_BuildPlan_GameId",
                table: "BuildPlan");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Quest");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "BuildPlan");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Quest",
                newName: "GameSaveId");

            migrationBuilder.RenameIndex(
                name: "IX_Quest_UserId",
                table: "Quest",
                newName: "IX_Quest_GameSaveId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BuildPlan",
                newName: "GameSaveId");

            migrationBuilder.RenameIndex(
                name: "IX_BuildPlan_UserId",
                table: "BuildPlan",
                newName: "IX_BuildPlan_GameSaveId");

            migrationBuilder.CreateTable(
                name: "BuildPlanQuest",
                columns: table => new
                {
                    BuildPlanId = table.Column<int>(type: "integer", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPlanQuest", x => new { x.BuildPlanId, x.QuestId });
                    table.ForeignKey(
                        name: "FK_BuildPlanQuest_BuildPlan_BuildPlanId",
                        column: x => x.BuildPlanId,
                        principalTable: "BuildPlan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BuildPlanQuest_Quest_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameSave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", unicode: false, maxLength: 250, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSave_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSave_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanQuest_QuestId",
                table: "BuildPlanQuest",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSave_GameId",
                table: "GameSave",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSave_UserId",
                table: "GameSave",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlan_GameSave_GameSaveId",
                table: "BuildPlan",
                column: "GameSaveId",
                principalTable: "GameSave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_GameSave_GameSaveId",
                table: "Quest",
                column: "GameSaveId",
                principalTable: "GameSave",
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
                name: "FK_Quest_GameSave_GameSaveId",
                table: "Quest");

            migrationBuilder.DropTable(
                name: "BuildPlanQuest");

            migrationBuilder.DropTable(
                name: "GameSave");

            migrationBuilder.RenameColumn(
                name: "GameSaveId",
                table: "Quest",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Quest_GameSaveId",
                table: "Quest",
                newName: "IX_Quest_UserId");

            migrationBuilder.RenameColumn(
                name: "GameSaveId",
                table: "BuildPlan",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BuildPlan_GameSaveId",
                table: "BuildPlan",
                newName: "IX_BuildPlan_UserId");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Quest",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "BuildPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BuildPlanResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BuildPlanId = table.Column<int>(type: "integer", nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: false),
                    SourceComponentId = table.Column<int>(type: "integer", nullable: false),
                    QuantityGathered = table.Column<int>(type: "integer", nullable: false),
                    TotalQuantityNeeded = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPlanResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildPlanResource_BuildPlan_BuildPlanId",
                        column: x => x.BuildPlanId,
                        principalTable: "BuildPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildPlanResource_Component_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildPlanResource_Component_SourceComponentId",
                        column: x => x.SourceComponentId,
                        principalTable: "Component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quest_GameId",
                table: "Quest",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlan_GameId",
                table: "BuildPlan",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_BuildPlanId",
                table: "BuildPlanResource",
                column: "BuildPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_ComponentId",
                table: "BuildPlanResource",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_SourceComponentId",
                table: "BuildPlanResource",
                column: "SourceComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlan_Game_GameId",
                table: "BuildPlan",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BuildPlan_User_UserId",
                table: "BuildPlan",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_Game_GameId",
                table: "Quest",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_User_UserId",
                table: "Quest",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
