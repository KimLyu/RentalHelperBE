using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RentHelper.Migrations
{
    public partial class RubuildInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cartitems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    uid = table.Column<Guid>(nullable: false),
                    pid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartitems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NickName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "verifycodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    VerityCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verifycodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    isSale = table.Column<bool>(nullable: false),
                    isRent = table.Column<bool>(nullable: false),
                    isExchange = table.Column<bool>(nullable: false),
                    Deposit = table.Column<int>(nullable: false),
                    Rent = table.Column<int>(nullable: false),
                    salePrice = table.Column<int>(nullable: false),
                    RentMethod = table.Column<string>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Type1 = table.Column<string>(nullable: false),
                    Type2 = table.Column<string>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    WeightPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wishItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ExchangeItem = table.Column<string>(nullable: true),
                    RequestQuantity = table.Column<int>(nullable: false),
                    WeightPoint = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wishItems_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    p_Title = table.Column<string>(nullable: true),
                    p_Desc = table.Column<string>(nullable: true),
                    p_Address = table.Column<string>(nullable: true),
                    p_isSale = table.Column<bool>(nullable: false),
                    p_isRent = table.Column<bool>(nullable: false),
                    p_isExchange = table.Column<bool>(nullable: false),
                    p_Deposit = table.Column<int>(nullable: false),
                    p_Rent = table.Column<int>(nullable: false),
                    p_salePrice = table.Column<int>(nullable: false),
                    p_RentMethod = table.Column<string>(nullable: true),
                    p_Type = table.Column<string>(nullable: true),
                    p_Type1 = table.Column<string>(nullable: true),
                    p_Type2 = table.Column<string>(nullable: true),
                    p_ownerId = table.Column<Guid>(nullable: false),
                    p_tradeCount = table.Column<int>(nullable: false),
                    TradeMethod = table.Column<int>(nullable: false),
                    TradeQuantity = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    OrderTime = table.Column<DateTime>(nullable: true),
                    PayTime = table.Column<DateTime>(nullable: true),
                    ProductSend = table.Column<DateTime>(nullable: true),
                    ProductArrive = table.Column<DateTime>(nullable: true),
                    ProductSendBack = table.Column<DateTime>(nullable: true),
                    ProductGetBack = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    Lender = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Desc = table.Column<string>(maxLength: 1500, nullable: true),
                    Path = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pictures_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    SenderName = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    createTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notes_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orderExchangeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    WishItemId = table.Column<Guid>(nullable: false),
                    packageQuantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderExchangeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orderExchangeItems_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orderExchangeItems_wishItems_WishItemId",
                        column: x => x.WishItemId,
                        principalTable: "wishItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notes_OrderId",
                table: "notes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_orderExchangeItems_OrderId",
                table: "orderExchangeItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_orderExchangeItems_WishItemId",
                table: "orderExchangeItems",
                column: "WishItemId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_ProductId",
                table: "orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_pictures_ProductId",
                table: "pictures",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_products_UserId",
                table: "products",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_wishItems_UserId",
                table: "wishItems",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cartitems");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "orderExchangeItems");

            migrationBuilder.DropTable(
                name: "pictures");

            migrationBuilder.DropTable(
                name: "verifycodes");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "wishItems");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
