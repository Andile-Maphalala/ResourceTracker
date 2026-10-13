using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerFacilityLevelAndRecipeFacilityLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredFacilityLevel",
                table: "Recipe",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "PlayerFacility",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredFacilityLevel",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "PlayerFacility");
        }
    }
}
