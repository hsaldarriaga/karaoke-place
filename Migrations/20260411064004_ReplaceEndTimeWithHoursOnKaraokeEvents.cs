using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace karaoke_place.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceEndTimeWithHoursOnKaraokeEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "KaraokeEvents",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                """
                UPDATE "KaraokeEvents"
                SET "Hours" = GREATEST(
                    1,
                    CEILING(EXTRACT(EPOCH FROM ("EndTime" - "StartTime")) / 3600.0)::integer
                )
                """);

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "KaraokeEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "KaraokeEvents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.Sql(
                """
                UPDATE "KaraokeEvents"
                SET "EndTime" = "StartTime" + make_interval(hours => "Hours")
                """);

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "KaraokeEvents");
        }
    }
}
