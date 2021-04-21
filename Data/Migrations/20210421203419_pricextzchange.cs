using Microsoft.EntityFrameworkCore.Migrations;

namespace dm.TCZ.Data.Migrations
{
    public partial class pricextzchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceXTZChange",
                table: "Prices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceXTZChangePct",
                table: "Prices",
                type: "decimal(12,8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceXTZChange",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "PriceXTZChangePct",
                table: "Prices");
        }
    }
}
