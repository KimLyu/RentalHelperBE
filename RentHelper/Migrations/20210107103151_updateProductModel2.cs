using Microsoft.EntityFrameworkCore.Migrations;

namespace RentHelper.Migrations
{
    public partial class updateProductModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeightPrice",
                table: "orders");

            migrationBuilder.AddColumn<decimal>(
                name: "p_WeightPrice",
                table: "orders",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "p_WeightPrice",
                table: "orders");

            migrationBuilder.AddColumn<decimal>(
                name: "WeightPrice",
                table: "orders",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
