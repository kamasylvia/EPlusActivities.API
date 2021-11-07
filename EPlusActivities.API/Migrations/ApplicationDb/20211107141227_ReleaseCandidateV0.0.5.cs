using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class ReleaseCandidateV005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GeneralLotteryRecords_DateTime",
                table: "GeneralLotteryRecords");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GeneralLotteryRecords_Channel_DateTime",
                table: "GeneralLotteryRecords",
                columns: new[] { "Channel", "DateTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_GeneralLotteryRecords_Channel_DateTime",
                table: "GeneralLotteryRecords");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLotteryRecords_DateTime",
                table: "GeneralLotteryRecords",
                column: "DateTime",
                unique: true);
        }
    }
}
