using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class AddStatement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statements",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        ActivityId = table.Column<Guid>(nullable: true),
                        DateTime = table.Column<DateTime>(nullable: false),
                        Draws = table.Column<int>(nullable: false),
                        Winners = table.Column<int>(nullable: false),
                        Redemption = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statements_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Statements_ActivityId",
                table: "Statements",
                column: "ActivityId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Statements");
        }
    }
}
