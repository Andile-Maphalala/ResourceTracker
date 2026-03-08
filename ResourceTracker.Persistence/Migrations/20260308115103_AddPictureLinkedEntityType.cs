using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureLinkedEntityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LinkedEntityType",
                table: "Picture",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedEntityType",
                table: "Picture");
        }
    }
}
