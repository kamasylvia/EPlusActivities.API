using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    public partial class InitialApplicationDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Limit = table.Column<int>(nullable: true),
                        DailyLimit = table.Column<int>(nullable: true),
                        RequiredCreditForRedeeming = table.Column<int>(nullable: false),
                        Name = table.Column<string>(nullable: false),
                        AvailableChannels = table.Column<string>(nullable: true),
                        ActivityType = table.Column<int>(nullable: false),
                        LotteryDisplay = table.Column<int>(nullable: false),
                        ActivityCode = table.Column<string>(nullable: true),
                        StartTime = table.Column<DateTime>(nullable: false),
                        EndTime = table.Column<DateTime>(nullable: true),
                        PrizeItemCount = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Name = table.Column<string>(maxLength: 256, nullable: true),
                        NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                        ConcurrencyStamp = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        UserName = table.Column<string>(maxLength: 256, nullable: true),
                        NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                        Email = table.Column<string>(maxLength: 256, nullable: true),
                        NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                        EmailConfirmed = table.Column<bool>(nullable: false),
                        PasswordHash = table.Column<string>(nullable: true),
                        SecurityStamp = table.Column<string>(nullable: true),
                        ConcurrencyStamp = table.Column<string>(nullable: true),
                        PhoneNumber = table.Column<string>(nullable: true),
                        PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                        TwoFactorEnabled = table.Column<bool>(nullable: false),
                        LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                        LockoutEnabled = table.Column<bool>(nullable: false),
                        AccessFailedCount = table.Column<int>(nullable: false),
                        RegisterDate = table.Column<DateTime>(nullable: false),
                        LastDrawDate = table.Column<DateTime>(nullable: true),
                        Credit = table.Column<int>(nullable: false),
                        IsMember = table.Column<bool>(nullable: false),
                        MemberId = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Name = table.Column<string>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Name = table.Column<string>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        UserId = table.Column<Guid>(nullable: false),
                        MemberId = table.Column<string>(nullable: false),
                        Points = table.Column<int>(nullable: false),
                        OldPoints = table.Column<int>(nullable: false),
                        NewPoints = table.Column<int>(nullable: false),
                        Reason = table.Column<string>(nullable: true),
                        SheetId = table.Column<string>(nullable: false),
                        RecordId = table.Column<string>(nullable: true),
                        UpdateType = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.Id);
                    table.UniqueConstraint("AK_Credits_SheetId", x => x.SheetId);
                }
            );

            migrationBuilder.CreateTable(
                name: "PrizeTiers",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Name = table.Column<string>(nullable: true),
                        Percentage = table.Column<int>(nullable: false),
                        ActivityId = table.Column<Guid>(nullable: false),
                        RequiredCredit = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrizeTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrizeTiers_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table =>
                    new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        RoleId = table.Column<Guid>(nullable: false),
                        ClaimType = table.Column<string>(nullable: true),
                        ClaimValue = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ActivityUserLinks",
                columns: table =>
                    new
                    {
                        UserId = table.Column<Guid>(nullable: false),
                        ActivityId = table.Column<Guid>(nullable: false),
                        UsedDraws = table.Column<int>(nullable: false),
                        TodayUsedDraws = table.Column<int>(nullable: false),
                        RemainingDraws = table.Column<int>(nullable: true),
                        AttendanceDays = table.Column<int>(nullable: true),
                        SequentialAttendanceDays = table.Column<int>(nullable: true),
                        LastAttendanceDate = table.Column<DateTime>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityUserLinks", x => new { x.ActivityId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ActivityUserLinks_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ActivityUserLinks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Recipient = table.Column<string>(nullable: true),
                        RecipientPhoneNumber = table.Column<string>(maxLength: 11, nullable: true),
                        Region = table.Column<string>(nullable: true),
                        Province = table.Column<string>(nullable: true),
                        City = table.Column<string>(nullable: true),
                        DetailedAddress = table.Column<string>(nullable: true),
                        Postcode = table.Column<string>(nullable: true),
                        UserId = table.Column<Guid>(nullable: false),
                        IsDefault = table.Column<bool>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table =>
                    new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        UserId = table.Column<Guid>(nullable: false),
                        ClaimType = table.Column<string>(nullable: true),
                        ClaimValue = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table =>
                    new
                    {
                        LoginProvider = table.Column<string>(nullable: false),
                        ProviderKey = table.Column<string>(nullable: false),
                        ProviderDisplayName = table.Column<string>(nullable: true),
                        UserId = table.Column<Guid>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_AspNetUserLogins",
                        x => new { x.LoginProvider, x.ProviderKey }
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table =>
                    new
                    {
                        UserId = table.Column<Guid>(nullable: false),
                        RoleId = table.Column<Guid>(nullable: false),
                        UserId1 = table.Column<Guid>(nullable: true),
                        RoleId1 = table.Column<Guid>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId1",
                        column: x => x.RoleId1,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table =>
                    new
                    {
                        UserId = table.Column<Guid>(nullable: false),
                        LoginProvider = table.Column<string>(nullable: false),
                        Name = table.Column<string>(nullable: false),
                        Value = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_AspNetUserTokens",
                        x => new { x.UserId, x.LoginProvider, x.Name }
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        ChannelCode = table.Column<int>(nullable: false),
                        EarnedCredits = table.Column<int>(nullable: false),
                        Date = table.Column<DateTime>(nullable: false),
                        UserId = table.Column<Guid>(nullable: true),
                        ActivityId = table.Column<Guid>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PrizeItems",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Name = table.Column<string>(nullable: false),
                        PrizeType = table.Column<int>(nullable: false),
                        CouponActiveCode = table.Column<string>(nullable: true),
                        CategoryId = table.Column<Guid>(nullable: true),
                        BrandId = table.Column<Guid>(nullable: true),
                        UnitPrice = table.Column<decimal>(nullable: true),
                        Credit = table.Column<int>(nullable: true),
                        Quantity = table.Column<int>(nullable: false),
                        Stock = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrizeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrizeItems_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_PrizeItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        Code = table.Column<string>(nullable: true),
                        Used = table.Column<bool>(nullable: false),
                        UserId = table.Column<Guid>(nullable: true),
                        PrizeItemId = table.Column<Guid>(nullable: true),
                        ActivityId = table.Column<Guid>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Coupons_PrizeItems_PrizeItemId",
                        column: x => x.PrizeItemId,
                        principalTable: "PrizeItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Coupons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "LotteryResults",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(nullable: false),
                        IsLucky = table.Column<bool>(nullable: false),
                        Delivered = table.Column<bool>(nullable: false),
                        PickedUp = table.Column<bool>(nullable: false),
                        PickedUpTime = table.Column<DateTime>(nullable: true),
                        ChannelCode = table.Column<int>(nullable: false),
                        LotteryDisplay = table.Column<int>(nullable: false),
                        Date = table.Column<DateTime>(nullable: false),
                        UserId = table.Column<Guid>(nullable: true),
                        ActivityId = table.Column<Guid>(nullable: true),
                        PrizeItemId = table.Column<Guid>(nullable: true),
                        PrizeTierId = table.Column<Guid>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotteryResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotteryResults_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_LotteryResults_PrizeItems_PrizeItemId",
                        column: x => x.PrizeItemId,
                        principalTable: "PrizeItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_LotteryResults_PrizeTiers_PrizeTierId",
                        column: x => x.PrizeTierId,
                        principalTable: "PrizeTiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_LotteryResults_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PrizeTierPrizeItems",
                columns: table =>
                    new
                    {
                        PrizeItemId = table.Column<Guid>(nullable: false),
                        PrizeTierId = table.Column<Guid>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PrizeTierPrizeItems",
                        x => new { x.PrizeTierId, x.PrizeItemId }
                    );
                    table.ForeignKey(
                        name: "FK_PrizeTierPrizeItems_PrizeItems_PrizeItemId",
                        column: x => x.PrizeItemId,
                        principalTable: "PrizeItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_PrizeTierPrizeItems_PrizeTiers_PrizeTierId",
                        column: x => x.PrizeTierId,
                        principalTable: "PrizeTiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityCode",
                table: "Activities",
                column: "ActivityCode",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ActivityUserLinks_UserId",
                table: "ActivityUserLinks",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId1",
                table: "AspNetUserRoles",
                column: "RoleId1"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1"
            );

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail"
            );

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_ActivityId",
                table: "AttendanceRecord",
                column: "ActivityId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_UserId",
                table: "AttendanceRecord",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ActivityId",
                table: "Coupons",
                column: "ActivityId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_PrizeItemId",
                table: "Coupons",
                column: "PrizeItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_UserId",
                table: "Coupons",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LotteryResults_ActivityId",
                table: "LotteryResults",
                column: "ActivityId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LotteryResults_PrizeItemId",
                table: "LotteryResults",
                column: "PrizeItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LotteryResults_PrizeTierId",
                table: "LotteryResults",
                column: "PrizeTierId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LotteryResults_UserId",
                table: "LotteryResults",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PrizeItems_BrandId",
                table: "PrizeItems",
                column: "BrandId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PrizeItems_CategoryId",
                table: "PrizeItems",
                column: "CategoryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PrizeTierPrizeItems_PrizeItemId",
                table: "PrizeTierPrizeItems",
                column: "PrizeItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PrizeTiers_ActivityId",
                table: "PrizeTiers",
                column: "ActivityId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ActivityUserLinks");

            migrationBuilder.DropTable(name: "Addresses");

            migrationBuilder.DropTable(name: "AspNetRoleClaims");

            migrationBuilder.DropTable(name: "AspNetUserClaims");

            migrationBuilder.DropTable(name: "AspNetUserLogins");

            migrationBuilder.DropTable(name: "AspNetUserRoles");

            migrationBuilder.DropTable(name: "AspNetUserTokens");

            migrationBuilder.DropTable(name: "AttendanceRecord");

            migrationBuilder.DropTable(name: "Coupons");

            migrationBuilder.DropTable(name: "Credits");

            migrationBuilder.DropTable(name: "LotteryResults");

            migrationBuilder.DropTable(name: "PrizeTierPrizeItems");

            migrationBuilder.DropTable(name: "AspNetRoles");

            migrationBuilder.DropTable(name: "AspNetUsers");

            migrationBuilder.DropTable(name: "PrizeItems");

            migrationBuilder.DropTable(name: "PrizeTiers");

            migrationBuilder.DropTable(name: "Brands");

            migrationBuilder.DropTable(name: "Categories");

            migrationBuilder.DropTable(name: "Activities");
        }
    }
}
