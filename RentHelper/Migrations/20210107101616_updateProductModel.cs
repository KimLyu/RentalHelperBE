using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RentHelper.Migrations
{
    public partial class updateProductModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "p_tradeCount",
                table: "orders");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "pictures",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightPrice",
                table: "orders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_pictures_OrderId",
                table: "pictures",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_pictures_orders_OrderId",
                table: "pictures",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pictures_orders_OrderId",
                table: "pictures");

            migrationBuilder.DropIndex(
                name: "IX_pictures_OrderId",
                table: "pictures");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "pictures");

            migrationBuilder.DropColumn(
                name: "WeightPrice",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "p_tradeCount",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
