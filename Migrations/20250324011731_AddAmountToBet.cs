using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballBettingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountToBet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BetAmount",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "BetTime",
                table: "Bets");

            migrationBuilder.RenameColumn(
                name: "Result",
                table: "Bets",
                newName: "TeamChosen");

            migrationBuilder.RenameColumn(
                name: "Prediction",
                table: "Bets",
                newName: "DatePlaced");

            migrationBuilder.RenameColumn(
                name: "IsSettled",
                table: "Bets",
                newName: "Amount");

            migrationBuilder.AlterColumn<int>(
                name: "MatchId",
                table: "Bets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeamChosen",
                table: "Bets",
                newName: "Result");

            migrationBuilder.RenameColumn(
                name: "DatePlaced",
                table: "Bets",
                newName: "Prediction");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Bets",
                newName: "IsSettled");

            migrationBuilder.AlterColumn<string>(
                name: "MatchId",
                table: "Bets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "BetAmount",
                table: "Bets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "BetTime",
                table: "Bets",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
