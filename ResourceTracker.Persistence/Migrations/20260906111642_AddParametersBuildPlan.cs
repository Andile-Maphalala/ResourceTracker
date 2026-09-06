using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddParametersBuildPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeFacilityRequirements",
                table: "BuildPlan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInventory",
                table: "BuildPlan",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeFacilityRequirements",
                table: "BuildPlan");

            migrationBuilder.DropColumn(
                name: "IncludeInventory",
                table: "BuildPlan");
        }
    }
}
