using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DbRestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Component_ParentComponentId",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Resource_ResourceId",
                table: "Recipe");

            migrationBuilder.DropTable(
                name: "ResourceCollection");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_ParentComponentId",
                table: "Recipe");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_ResourceId",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "ParentComponentId",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Recipe");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Recipe",
                newName: "AmountMade");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "QuestComponents",
                newName: "AmountRequired");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "Recipe",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AmountAquired",
                table: "QuestComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Component",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RecipeComponent",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    AmountRequired = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeComponent", x => new { x.RecipeId, x.ComponentId });
                    table.ForeignKey(
                        name: "FK_RecipeComponent_Component_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeComponent_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeComponent_ComponentId",
                table: "RecipeComponent",
                column: "ComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeComponent");

            migrationBuilder.DropColumn(
                name: "AmountAquired",
                table: "QuestComponents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Component");

            migrationBuilder.RenameColumn(
                name: "AmountMade",
                table: "Recipe",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "AmountRequired",
                table: "QuestComponents",
                newName: "Quantity");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "Recipe",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ParentComponentId",
                table: "Recipe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResourceId",
                table: "Recipe",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestId = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    ItemsGathered = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceCollection_Quest_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceCollection_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_ParentComponentId",
                table: "Recipe",
                column: "ParentComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_ResourceId",
                table: "Recipe",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCollection_QuestId",
                table: "ResourceCollection",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCollection_ResourceId",
                table: "ResourceCollection",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Component_ParentComponentId",
                table: "Recipe",
                column: "ParentComponentId",
                principalTable: "Component",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Resource_ResourceId",
                table: "Recipe",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
