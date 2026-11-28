using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bateka.games.Data.Migrations
{
    /// <inheritdoc />
    public partial class MastermindResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowRepeats",
                table: "GameResults",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Colours",
                table: "GameResults",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Columns",
                table: "GameResults",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuessLimit",
                table: "GameResults",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowRepeats",
                table: "GameResults");

            migrationBuilder.DropColumn(
                name: "Colours",
                table: "GameResults");

            migrationBuilder.DropColumn(
                name: "Columns",
                table: "GameResults");

            migrationBuilder.DropColumn(
                name: "GuessLimit",
                table: "GameResults");
        }
    }
}
