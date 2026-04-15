using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace karaoke_place.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatesToKaraokeEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Coordinates",
                table: "KaraokeEvents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "KaraokeEvents");
        }
    }
}
