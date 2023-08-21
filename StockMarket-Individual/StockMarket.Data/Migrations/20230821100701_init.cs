using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    Quantity = table.Column<decimal>(type: "Money", nullable: false),
                    TradeSide = table.Column<int>(type: "int", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    BuyOrderId = table.Column<long>(type: "bigint", nullable: false),
                    SellOrderId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "Money", nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_Orders_BuyOrderId",
                        column: x => x.BuyOrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Orders_SellOrderId",
                        column: x => x.SellOrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_BuyOrderId",
                table: "Trades",
                column: "BuyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_SellOrderId",
                table: "Trades",
                column: "SellOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
