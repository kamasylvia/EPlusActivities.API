using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class DailyRedemptionLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastDrawDate", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "DailyLimit", table: "Activities");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TodayUsedRedempion",
                table: "ActivityUserLinks",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "DailyDrawLimit",
                table: "Activities",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "DailyRedemptionLimit",
                table: "Activities",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLotteryRecords_DateTime",
                table: "GeneralLotteryRecords",
                column: "DateTime",
                unique: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeneralLotteryRecords_DateTime",
                table: "GeneralLotteryRecords"
            );

            migrationBuilder.DropColumn(name: "LastLoginDate", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "TodayUsedRedempion", table: "ActivityUserLinks");

            migrationBuilder.DropColumn(name: "DailyDrawLimit", table: "Activities");

            migrationBuilder.DropColumn(name: "DailyRedemptionLimit", table: "Activities");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDrawDate",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "DailyLimit",
                table: "Activities",
                type: "int",
                nullable: true
            );
        }
    }
}
