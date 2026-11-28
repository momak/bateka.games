using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bateka.games.Data.Migrations
{
    /// <inheritdoc />
    public partial class Matches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Game = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatorId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OpponentId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FirstPlayerId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Moves = table.Column<List<int>>(type: "integer[]", nullable: false),
                    MoveSeconds = table.Column<int>(type: "integer", nullable: false),
                    MoveDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WinnerId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EndReason = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CreatorId_Status",
                table: "Matches",
                columns: new[] { "CreatorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_OpponentId_Status",
                table: "Matches",
                columns: new[] { "OpponentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Status_CreatedAt",
                table: "Matches",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Status_MoveDeadline",
                table: "Matches",
                columns: new[] { "Status", "MoveDeadline" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
