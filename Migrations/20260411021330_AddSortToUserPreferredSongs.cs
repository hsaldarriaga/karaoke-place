using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace karaoke_place.Migrations
{
    /// <inheritdoc />
    public partial class AddSortToUserPreferredSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "UserPreferredSongs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                table: "UserPreferredSongs");
        }
    }
}
