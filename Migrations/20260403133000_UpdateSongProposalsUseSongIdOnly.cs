using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using karaoke_place.Data;

#nullable disable

namespace karaoke_place.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260403133000_UpdateSongProposalsUseSongIdOnly")]
    public partial class UpdateSongProposalsUseSongIdOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"SongProposals\" WHERE \"SongId\" IS NULL;");

            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "SongProposals");

            migrationBuilder.DropColumn(
                name: "SongTitle",
                table: "SongProposals");

            migrationBuilder.AlterColumn<int>(
                name: "SongId",
                table: "SongProposals",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_SongProposals_Songs_SongId",
                table: "SongProposals");

            migrationBuilder.AddForeignKey(
                name: "FK_SongProposals_Songs_SongId",
                table: "SongProposals",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SongProposals_Songs_SongId",
                table: "SongProposals");

            migrationBuilder.AlterColumn<int>(
                name: "SongId",
                table: "SongProposals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "SongProposals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SongTitle",
                table: "SongProposals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SongProposals_Songs_SongId",
                table: "SongProposals",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
