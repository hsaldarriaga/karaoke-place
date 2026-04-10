using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace karaoke_place.Migrations
{
    /// <inheritdoc />
    public partial class AddAuth0SubjectToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth0Subject",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Auth0Subject",
                table: "Users",
                column: "Auth0Subject",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Auth0Subject",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Auth0Subject",
                table: "Users");
        }
    }
}
