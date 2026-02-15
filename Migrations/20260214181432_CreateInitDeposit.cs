using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryCRM.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Deposits",
                columns: new[] { "Id", "Name", "UserId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "Main Deposit", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Deposits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));
        }
    }
}
