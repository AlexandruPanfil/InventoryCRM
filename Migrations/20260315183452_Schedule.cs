using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryCRM.Migrations
{
    /// <inheritdoc />
    public partial class Schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ScheduleId",
                table: "Orders",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Schedules_ScheduleId",
                table: "Orders",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Schedules_ScheduleId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ScheduleId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Orders");
        }
    }
}
