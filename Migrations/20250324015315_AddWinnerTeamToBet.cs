using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballBettingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerTeamToBet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WinnerTeam",
                table: "Bets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerTeam",
                table: "Bets");
        }
    }
}
