using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class ApplicationDbMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Date", table: "LotteryResults");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "LotteryResults",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DateTime", table: "LotteryResults");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "LotteryResults",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );
        }
    }
}
