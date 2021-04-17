using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.TCZ.Data.Migrations
{
    public partial class request : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_Response_Type",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "Requests",
                newName: "DiscordUserName");

            migrationBuilder.AddColumn<string>(
                name: "Command",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscordUserId",
                table: "Requests",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsRateLimited",
                table: "Requests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Response_Command",
                table: "Requests",
                columns: new[] { "Response", "Command" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_Response_Command",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Command",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "DiscordUserId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "IsRateLimited",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "DiscordUserName",
                table: "Requests",
                newName: "User");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Response_Type",
                table: "Requests",
                columns: new[] { "Response", "Type" });
        }
    }
}
