using Microsoft.EntityFrameworkCore.Migrations;

namespace LinnworksTest.Migrations
{
    public partial class stocklevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockLevel",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockLevel",
                table: "Products");
        }
    }
}
