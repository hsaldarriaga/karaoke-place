using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using karaoke_place.Data;

#nullable disable

namespace karaoke_place.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260403130000_AddUserPreferredSongs")]
    public partial class AddUserPreferredSongs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferredSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SongId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferredSongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferredSongs_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPreferredSongs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferredSongs_SongId",
                table: "UserPreferredSongs",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferredSongs_UserId",
                table: "UserPreferredSongs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferredSongs_UserId_SongId",
                table: "UserPreferredSongs",
                columns: new[] { "UserId", "SongId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferredSongs");
        }
    }
}
