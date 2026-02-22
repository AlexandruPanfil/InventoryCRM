using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryCRM.Migrations
{
    /// <inheritdoc />
    public partial class CustomersAndOrdersChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Orders_OrdersId",
                table: "Units");

            migrationBuilder.RenameColumn(
                name: "OrdersId",
                table: "Units",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Units_OrdersId",
                table: "Units",
                newName: "IX_Units_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Orders_OrderId",
                table: "Units",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Orders_OrderId",
                table: "Units");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Units",
                newName: "OrdersId");

            migrationBuilder.RenameIndex(
                name: "IX_Units_OrderId",
                table: "Units",
                newName: "IX_Units_OrdersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Orders_OrdersId",
                table: "Units",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
