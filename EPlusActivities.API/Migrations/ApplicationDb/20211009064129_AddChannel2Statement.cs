using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class AddChannel2Statement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "Statements",
                nullable: false,
                defaultValue: 0
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Channel", table: "Statements");
        }
    }
}
