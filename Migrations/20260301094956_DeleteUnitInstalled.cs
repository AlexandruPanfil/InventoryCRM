using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryCRM.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUnitInstalled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Orders_OrderId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "UnitsInstalled");

            migrationBuilder.DropTable(
                name: "UnitsReserved");

            migrationBuilder.DropIndex(
                name: "IX_Units_OrderId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Units");

            migrationBuilder.CreateTable(
                name: "UnitsAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitsAssignment_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitsAssignment_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitsAssignment_CustomerId",
                table: "UnitsAssignment",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsAssignment_OrderId",
                table: "UnitsAssignment",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitsAssignment");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Units",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UnitsInstalled",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsInstalled", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitsInstalled_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitsInstalled_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UnitsReserved",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsReserved", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitsReserved_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitsReserved_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_OrderId",
                table: "Units",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsInstalled_CustomerId",
                table: "UnitsInstalled",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsInstalled_OrderId",
                table: "UnitsInstalled",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsReserved_CustomerId",
                table: "UnitsReserved",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsReserved_OrderId",
                table: "UnitsReserved",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Orders_OrderId",
                table: "Units",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
