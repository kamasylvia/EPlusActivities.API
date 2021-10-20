using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileService.Data.Migrations.ApplicationDb
{
    public partial class AppDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                    name: "Files",
                    columns: table =>
                        new
                        {
                            Id = table.Column<Guid>(
                                type: "char(36)",
                                nullable: false,
                                collation: "ascii_general_ci"
                            ),
                            OwnerId = table.Column<Guid>(
                                type: "char(36)",
                                nullable: false,
                                collation: "ascii_general_ci"
                            ),
                            Key = table.Column<string>(type: "varchar(255)", nullable: false)
                                .Annotation("MySql:CharSet", "utf8mb4"),
                            ContentType = table.Column<string>(type: "longtext", nullable: false)
                                .Annotation("MySql:CharSet", "utf8mb4"),
                            FilePath = table.Column<string>(type: "longtext", nullable: true)
                                .Annotation("MySql:CharSet", "utf8mb4")
                        },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Files", x => x.Id);
                        table.UniqueConstraint(
                            "AK_Files_OwnerId_Key",
                            x => new { x.OwnerId, x.Key }
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Files");
        }
    }
}
