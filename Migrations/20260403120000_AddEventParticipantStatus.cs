using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using karaoke_place.Data;

#nullable disable

namespace karaoke_place.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260403120000_AddEventParticipantStatus")]
    public partial class AddEventParticipantStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventParticipants");
        }
    }
}
