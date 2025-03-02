using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BuidPlanChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountRequired",
                table: "QuestComponents");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Quest",
                type: "varchar(250)",
                unicode: false,
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BuildPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildPlan_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildPlanComponent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    QuantityNeeded = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    BuildPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPlanComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildPlanComponent_BuildPlan_BuildPlanId",
                        column: x => x.BuildPlanId,
                        principalTable: "BuildPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildPlanComponent_Component_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildPlanResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalQuantityNeeded = table.Column<int>(type: "int", nullable: false),
                    QuantityGathered = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    SourceComponentId = table.Column<int>(type: "int", nullable: false),
                    BuildPlanId = table.Column<int>(type: "int", nullable: false),
                    ComponentId1 = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_BuildPlanResource_Component_ComponentId1",
                        column: x => x.ComponentId1,
                        principalTable: "Component",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BuildPlanResource_Component_SourceComponentId",
                        column: x => x.SourceComponentId,
                        principalTable: "Component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlan_UserId",
                table: "BuildPlan",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanComponent_BuildPlanId",
                table: "BuildPlanComponent",
                column: "BuildPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanComponent_ComponentId",
                table: "BuildPlanComponent",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_BuildPlanId",
                table: "BuildPlanResource",
                column: "BuildPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_ComponentId",
                table: "BuildPlanResource",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_ComponentId1",
                table: "BuildPlanResource",
                column: "ComponentId1");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlanResource_SourceComponentId",
                table: "BuildPlanResource",
                column: "SourceComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildPlanComponent");

            migrationBuilder.DropTable(
                name: "BuildPlanResource");

            migrationBuilder.DropTable(
                name: "BuildPlan");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Quest");

            migrationBuilder.AddColumn<int>(
                name: "AmountRequired",
                table: "QuestComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
