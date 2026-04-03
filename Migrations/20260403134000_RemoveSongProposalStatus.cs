using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using karaoke_place.Data;

#nullable disable

namespace karaoke_place.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260403134000_RemoveSongProposalStatus")]
    public partial class RemoveSongProposalStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SongProposals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SongProposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
