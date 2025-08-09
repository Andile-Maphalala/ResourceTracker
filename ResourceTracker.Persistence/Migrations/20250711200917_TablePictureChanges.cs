using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ResourceTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TablePictureChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Quest");

            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "Quest",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "Game",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "Component",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    AltText = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quest_PictureId",
                table: "Quest",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_PictureId",
                table: "Game",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Component_PictureId",
                table: "Component",
                column: "PictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Picture_PictureId",
                table: "Component",
                column: "PictureId",
                principalTable: "Picture",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Picture_PictureId",
                table: "Game",
                column: "PictureId",
                principalTable: "Picture",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Quest_Picture_PictureId",
                table: "Quest",
                column: "PictureId",
                principalTable: "Picture",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Picture_PictureId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Picture_PictureId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Quest_Picture_PictureId",
                table: "Quest");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropIndex(
                name: "IX_Quest_PictureId",
                table: "Quest");

            migrationBuilder.DropIndex(
                name: "IX_Game_PictureId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Component_PictureId",
                table: "Component");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Quest");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Component");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Quest",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CoverImage",
                table: "Game",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
