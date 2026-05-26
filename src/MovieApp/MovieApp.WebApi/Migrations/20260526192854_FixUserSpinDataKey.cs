using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixUserSpinDataKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserSpinData");

            migrationBuilder.CreateTable(
                name: "UserSpinData",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BonusSpins = table.Column<int>(type: "int", nullable: false),
                    DailySpinsRemaining = table.Column<int>(type: "int", nullable: false),
                    EventSpinRewardsToday = table.Column<int>(type: "int", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSlotSpinReset = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTriviaSpinReset = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginStreak = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpinData", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserSpinData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    RewardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RewardType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedemptionStatus = table.Column<bool>(type: "bit", nullable: false),
                    ApplicabilityScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.RewardId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserSpinData_Users_UserId",
                table: "UserSpinData",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
        name: "FK_UserSpinData_Users_UserId",
        table: "UserSpinData");

            migrationBuilder.DropTable(name: "FavoriteEvents");
            migrationBuilder.DropTable(name: "Rewards");
            migrationBuilder.DropTable(name: "UserSpinData");

            migrationBuilder.CreateTable(
                name: "UserSpinData",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonusSpins = table.Column<int>(type: "int", nullable: false),
                    DailySpinsRemaining = table.Column<int>(type: "int", nullable: false),
                    EventSpinRewardsToday = table.Column<int>(type: "int", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSlotSpinReset = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTriviaSpinReset = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginStreak = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpinData", x => x.UserId);
                });
        }
    }
}
