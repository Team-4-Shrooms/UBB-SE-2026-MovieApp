using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MovieApp.WebApi.Data;

#nullable disable

namespace MovieApp.WebApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260527000000_FixUserSpinDataIdentity")]
    public partial class FixUserSpinDataIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // UserId was incorrectly created as IDENTITY(1,1) -- it is a FK to Users and must accept explicit values.
            migrationBuilder.Sql(@"
                BEGIN TRANSACTION;

                CREATE TABLE [UserSpinData_New] (
                    [UserId]                 INT        NOT NULL,
                    [BonusSpins]             INT        NOT NULL,
                    [DailySpinsRemaining]    INT        NOT NULL,
                    [EventSpinRewardsToday]  INT        NOT NULL,
                    [LastLoginDate]          DATETIME2  NOT NULL,
                    [LastSlotSpinReset]      DATETIME2  NOT NULL,
                    [LastTriviaSpinReset]    DATETIME2  NOT NULL,
                    [LoginStreak]            INT        NOT NULL,
                    CONSTRAINT [PK_UserSpinData_New] PRIMARY KEY CLUSTERED ([UserId] ASC)
                );

                IF EXISTS (SELECT 1 FROM [UserSpinData])
                BEGIN
                    INSERT INTO [UserSpinData_New] ([UserId], [BonusSpins], [DailySpinsRemaining], [EventSpinRewardsToday], [LastLoginDate], [LastSlotSpinReset], [LastTriviaSpinReset], [LoginStreak])
                    SELECT [UserId], [BonusSpins], [DailySpinsRemaining], [EventSpinRewardsToday], [LastLoginDate], [LastSlotSpinReset], [LastTriviaSpinReset], [LoginStreak] FROM [UserSpinData];
                END

                DROP TABLE [UserSpinData];

                EXEC sp_rename N'dbo.UserSpinData_New', N'UserSpinData', N'OBJECT';
                EXEC sp_rename N'PK_UserSpinData_New', N'PK_UserSpinData', N'OBJECT';

                COMMIT;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                BEGIN TRANSACTION;

                CREATE TABLE [UserSpinData_New] (
                    [UserId]                 INT        NOT NULL IDENTITY(1,1),
                    [BonusSpins]             INT        NOT NULL,
                    [DailySpinsRemaining]    INT        NOT NULL,
                    [EventSpinRewardsToday]  INT        NOT NULL,
                    [LastLoginDate]          DATETIME2  NOT NULL,
                    [LastSlotSpinReset]      DATETIME2  NOT NULL,
                    [LastTriviaSpinReset]    DATETIME2  NOT NULL,
                    [LoginStreak]            INT        NOT NULL,
                    CONSTRAINT [PK_UserSpinData] PRIMARY KEY CLUSTERED ([UserId] ASC)
                );

                DROP TABLE [UserSpinData];

                EXEC sp_rename N'dbo.UserSpinData_New', N'UserSpinData', N'OBJECT';

                COMMIT;
            ");
        }
    }
}
