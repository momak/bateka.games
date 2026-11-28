using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bateka.games.Data.Migrations
{
    /// <inheritdoc />
    public partial class MatchInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitedId",
                table: "Matches",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RematchId",
                table: "Matches",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_InvitedId_Status",
                table: "Matches",
                columns: new[] { "InvitedId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matches_InvitedId_Status",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "InvitedId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "RematchId",
                table: "Matches");
        }
    }
}
