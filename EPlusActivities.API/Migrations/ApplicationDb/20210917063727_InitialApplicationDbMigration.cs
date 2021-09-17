using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class InitialApplicationDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastLoginDate", table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDrawDate",
                table: "AspNetUsers",
                nullable: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastDrawDate", table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true
            );
        }
    }
}
