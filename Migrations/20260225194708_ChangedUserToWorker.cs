using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryCRM.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUserToWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                newName: "IX_Orders_WorkerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Deposits",
                newName: "WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_UserId",
                table: "Deposits",
                newName: "IX_Deposits_WorkerId");

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Workername = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Workers_WorkerId",
                table: "Deposits",
                column: "WorkerId",
                principalTable: "Workers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Workers_WorkerId",
                table: "Orders",
                column: "WorkerId",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Workers_WorkerId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Workers_WorkerId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_WorkerId",
                table: "Orders",
                newName: "IX_Orders_UserId");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "Deposits",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_WorkerId",
                table: "Deposits",
                newName: "IX_Deposits_UserId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
