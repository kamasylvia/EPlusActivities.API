using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class AddDetailedLotteryStatement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralLotteryRecords_Activities_ActivityId",
                table: "GeneralLotteryRecords"
            );

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GeneralLotteryRecords_Channel_DateTime",
                table: "GeneralLotteryRecords"
            );

            migrationBuilder.DropIndex(
                name: "IX_GeneralLotteryRecords_ActivityId",
                table: "GeneralLotteryRecords"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "ActivityId",
                table: "GeneralLotteryRecords",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true
            );

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GeneralLotteryRecords_ActivityId_Channel_DateTime",
                table: "GeneralLotteryRecords",
                columns: new[] { "ActivityId", "Channel", "DateTime" }
            );

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralLotteryRecords_Activities_ActivityId",
                table: "GeneralLotteryRecords",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralLotteryRecords_Activities_ActivityId",
                table: "GeneralLotteryRecords"
            );

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GeneralLotteryRecords_ActivityId_Channel_DateTime",
                table: "GeneralLotteryRecords"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "ActivityId",
                table: "GeneralLotteryRecords",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid)
            );

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GeneralLotteryRecords_Channel_DateTime",
                table: "GeneralLotteryRecords",
                columns: new[] { "Channel", "DateTime" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLotteryRecords_ActivityId",
                table: "GeneralLotteryRecords",
                column: "ActivityId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralLotteryRecords_Activities_ActivityId",
                table: "GeneralLotteryRecords",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
