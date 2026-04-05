using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using karaoke_place.Data;

#nullable disable

namespace karaoke_place.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260405090000_AllowDuplicateSongProposalOrder")]
    public partial class AllowDuplicateSongProposalOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongProposals_EventId_Order",
                table: "SongProposals");

            migrationBuilder.CreateIndex(
                name: "IX_SongProposals_EventId_Order",
                table: "SongProposals",
                columns: new[] { "EventId", "Order" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SongProposals_EventId_Order",
                table: "SongProposals");

            migrationBuilder.CreateIndex(
                name: "IX_SongProposals_EventId_Order",
                table: "SongProposals",
                columns: new[] { "EventId", "Order" },
                unique: true);
        }
    }
}
