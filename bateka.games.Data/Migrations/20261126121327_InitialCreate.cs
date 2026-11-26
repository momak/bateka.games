using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bateka.games.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Game = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Outcome = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Pairs = table.Column<int>(type: "integer", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    Length = table.Column<int>(type: "integer", nullable: true),
                    Moves = table.Column<int>(type: "integer", nullable: true),
                    DurationMs = table.Column<int>(type: "integer", nullable: true),
                    PlayedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_Game_Difficulty",
                table: "GameResults",
                columns: new[] { "Game", "Difficulty" });

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_Game_Pairs",
                table: "GameResults",
                columns: new[] { "Game", "Pairs" });

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_PlayedAt",
                table: "GameResults",
                column: "PlayedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_UserId_Game",
                table: "GameResults",
                columns: new[] { "UserId", "Game" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameResults");
        }
    }
}
