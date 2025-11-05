using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomCast.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailsSupportToAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlbumScreenAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlbumId = table.Column<int>(type: "INTEGER", nullable: false),
                    ScreenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumScreenAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumScreenAssignments_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "AlbumId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumScreenAssignments_Screens_ScreenId",
                        column: x => x.ScreenId,
                        principalTable: "Screens",
                        principalColumn: "ScreenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumScreenAssignments_AlbumId",
                table: "AlbumScreenAssignments",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumScreenAssignments_ScreenId",
                table: "AlbumScreenAssignments",
                column: "ScreenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumScreenAssignments");
        }
    }
}
